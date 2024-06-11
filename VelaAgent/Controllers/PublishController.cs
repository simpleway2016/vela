using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelaAgent.DBModels;
using VelaAgent.Infrastructures;
using VelaLib;
using VelaLib.Dtos;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Way.EntityDB;
using System.Security.Cryptography;
using VelaAgent.Dtos;
using Way.Lib;
using Docker.DotNet.Models;
using System.Data;
using System;
using VelaAgent.KeepAlive;

namespace VelaAgent.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PublishController : ControllerBase
    {
        private readonly SysDBContext _db;
        private readonly IProcessService _processService;
        private readonly ICmdRunner _cmdRunner;
        private readonly ILogger<PublishController> _logger;
        private readonly IDockerEngine _dockerEngine;
        private readonly ProjectBackup _projectBackup;
        private readonly ProjectTtyWorker _projectTtyWorker;
        private readonly IFileService _fileService;
        private readonly KeepProcessAliveFactory _keepProcessAliveFactory;

        public PublishController(SysDBContext db, IProcessService processService, ICmdRunner cmdRunner,
            ILogger<PublishController> logger, IDockerEngine dockerEngine,
            ProjectBackup projectBackup,
            ProjectTtyWorker projectTtyWorker,
            IFileService fileService,
            KeepProcessAliveFactory keepProcessAliveFactory)
        {
            _db = db;
            _processService = processService;
            _cmdRunner = cmdRunner;
            _logger = logger;
            _dockerEngine = dockerEngine;
            _projectBackup = projectBackup;
            _projectTtyWorker = projectTtyWorker;
            _fileService = fileService;
            _keepProcessAliveFactory = keepProcessAliveFactory;
        }

        /// <summary>
        /// 是否已经设置客户端证书指纹
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public bool HasSetClientCert()
        {
            return string.IsNullOrEmpty(Global.ClientCertHash) == false;
        }

        [HttpGet]
        public string GetVersion()
        {
            return typeof(PublishController).Assembly.GetName().Version.ToString();
        }

        [HttpGet]
        public void SetClientCert(string certHash)
        {
            Global.ClientCertHash = certHash;
        }

        /// <summary>
        /// 重启本服务
        /// </summary>
        [HttpGet]
        public void Restart()
        {
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// 新增部署服务
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        [HttpPost]
        public string AddProject([FromBody] Project project)
        {
            if (string.IsNullOrWhiteSpace(project.Name))
                throw new ServiceException("名称为空");

            project.ProcessId = null;
            project.DockerContainerId = null;
            project.IsFirstUpload = true;
            bool containsSpecialCharacters = Regex.IsMatch(project.Name, @"[^a-z0-9A-Z\._]");

            if (containsSpecialCharacters)
            {
                throw new ServiceException("名称只能由数字、字母、下划线和点构成");
            }
            if (!string.IsNullOrWhiteSpace(project.GitUrl) && string.IsNullOrWhiteSpace(project.BranchName))
            {
                throw new ServiceException("请填写分支名称");
            }

            try
            {
                if (string.IsNullOrWhiteSpace(project.PublishPath))
                {
                    var folder = Path.Combine(Global.AppConfig.Current.PublishRootPath, project.Name);
                    if (Directory.Exists(folder) == false)
                    {
                        Directory.CreateDirectory(folder);
                    }
                }
                else
                {
                    var path = project.PublishPath.Trim();
                    if (OperatingSystem.IsWindows())
                    {
                        if (path[1] != ':')
                        {
                            throw new ServiceException("部署路径必须是绝对路径");
                        }
                    }
                    else
                    {
                        if (path.StartsWith("/") == false)
                        {
                            throw new ServiceException("部署路径必须是绝对路径");
                        }
                    }

                    if (Directory.Exists(path) == false)
                    {
                        Directory.CreateDirectory(path);
                    }
                }

                project.Guid = Guid.NewGuid().ToString("N");
                _db.Insert(project);

                return project.Guid;

            }
            catch (RepeatException)
            {
                throw new ServiceException("项目名称重复");
            }

        }

        [HttpPost]
        public async Task ModifyProject([FromBody] Project project)
        {
            if (string.IsNullOrWhiteSpace(project.Name))
                throw new ServiceException("名称为空");

            bool containsSpecialCharacters = Regex.IsMatch(project.Name, @"[^a-z0-9A-Z\._]");

            if (containsSpecialCharacters)
            {
                throw new ServiceException("名称只能由数字、字母、下划线和点构成");
            }

            try
            {
                if (string.IsNullOrWhiteSpace(project.PublishPath))
                {
                    var folder = Path.Combine(Global.AppConfig.Current.PublishRootPath, project.Name);
                    if (Directory.Exists(folder) == false)
                    {
                        Directory.CreateDirectory(folder);
                    }
                }
                else
                {
                    var path = project.PublishPath.Trim();
                    if (OperatingSystem.IsWindows())
                    {
                        if (path[1] != ':')
                        {
                            throw new ServiceException("部署路径必须是绝对路径");
                        }
                    }
                    else
                    {
                        if (path.StartsWith("/") == false)
                        {
                            throw new ServiceException("部署路径必须是绝对路径");
                        }
                    }

                    if (Directory.Exists(path) == false)
                    {
                        Directory.CreateDirectory(path);
                    }
                }

                var data = await _db.Project.FirstOrDefaultAsync(m => m.Guid == project.Guid);
                project.CopyValueTo(data, true, false);

                //去除不更新的字段
                data.ChangedProperties.Remove(nameof(project.UserId));
                data.ChangedProperties.Remove(nameof(project.ProcessId));
                data.ChangedProperties.Remove(nameof(project.DockerContainerId));
                data.ChangedProperties.Remove(nameof(project.IsStopped));

                var changeitem = data.ChangedProperties.Where(m => m.Key == nameof(data.RunType)).Select(m => m.Value).FirstOrDefault();

                if (changeitem != null)
                {
                    //RunType设定如果改变，那么，停止程序
                    //这里不能加await，否则_db内部的数据库对象会被Dispose掉，不知道为啥
                    await StopProject(project.Guid);

                    var item = _keepProcessAliveFactory.Create(project.Guid);
                    if (item != null)
                    {
                        await item.DeleteProject();
                    }
                }

                changeitem = data.ChangedProperties.Where(m => m.Key == nameof(data.DockerPortMap)).Select(m => m.Value).FirstOrDefault();
                if (changeitem != null)
                {
                    //如果DockerPortMap变化，认为和文件改变一个意思
                    data.FileHasUpdated = true;
                }

                _db.Update(data);
                _keepProcessAliveFactory.UpdateProject(data);

            }
            catch (RepeatException)
            {
                throw new ServiceException("项目名称重复");
            }

        }

        [HttpGet]
        public async Task DeleteProject(string guid)
        {
            _db.Delete<Project>(m => m.Guid == guid);
            KeepProcessAlive keepObj = null;

            try
            {
                 keepObj = _keepProcessAliveFactory.Create(guid);
            }
            catch
            {
            }
          
            if (keepObj != null)
            {
                await keepObj.Stop();
                await keepObj.DeleteProject();

                _keepProcessAliveFactory.RemoveCache(guid);
            }
        }

        /// <summary>
        /// 获取所有部署的程序服务
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Project[] GetAllProjects()
        {
            return _db.Project.OrderBy(m => m.Name).ToArray();
        }

        [HttpGet]
        public Project[] GetProjectsByGitUrl(string gitUrl)
        {
            return _db.Project.Where(m => m.GitUrl == gitUrl).ToArray();
        }

        [HttpPost]
        public async Task<string[]> GetNeedUploadFiles([FromBody] List<UploadFileInfo> fileInfos, [FromQuery] string projectGuid)
        {
            try
            {
                var project = await _db.Project.FirstOrDefaultAsync(m => m.Guid == projectGuid);
                if (project == null)
                    throw new ServiceException("此程序服务不存在");


                var publishPath = Path.Combine(Global.AppConfig.Current.PublishRootPath, project.Name);
                if (!string.IsNullOrWhiteSpace(project.PublishPath))
                {
                    publishPath = project.PublishPath;
                }

                if (Directory.Exists(publishPath) == false)
                {
                    try
                    {
                        Directory.CreateDirectory(publishPath);
                    }
                    catch (Exception ex)
                    {
                        throw new ServiceException($"创建目录{publishPath}发生错误，{ex.Message}");
                    }
                }

                var configFiles = project.ConfigFiles?.Split(',');
                var excludeFiles = project.ExcludeFiles?.Split(',');
                var otherProjects = _db.Project.Where(m => m.Guid != projectGuid).ToArray();

                for (int i = 0; i < fileInfos.Count; i++)
                {
                    var fileinfo = fileInfos[i];
                    var localFilePath = Path.Combine(publishPath, fileinfo.Path);
                    if (configFiles != null && configFiles.Any(m => string.Equals(m, fileinfo.Path, StringComparison.OrdinalIgnoreCase)))
                    {
                        //配置文件如果已经存在，则忽略
                        if (System.IO.File.Exists(localFilePath))
                        {
                            fileInfos.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }

                    if (excludeFiles != null && excludeFiles.Any(m => string.Equals(m, fileinfo.Path, StringComparison.OrdinalIgnoreCase)))
                    {
                        //排除的文件如果已经存在，则忽略
                        if (System.IO.File.Exists(localFilePath))
                        {
                            fileInfos.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }

                    if (System.IO.File.Exists(localFilePath) && new FileInfo(localFilePath).Length == fileinfo.Length)
                    {
                        //文件存在，并且md5一样，不用上传
                        if (await SysUtility.CalculateMD5Async(localFilePath) == fileinfo.MD5)
                        {
                            fileInfos.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }

                    var uploadToPath = Path.Combine(publishPath, "$$$publish_agent_update", fileinfo.Path);
                    var uploadToFolder = Path.GetDirectoryName(uploadToPath);
                    if (Directory.Exists(uploadToFolder) == false)
                    {
                        Directory.CreateDirectory(uploadToFolder);
                    }

                    if (System.IO.File.Exists(uploadToPath) && new FileInfo(uploadToPath).Length == fileinfo.Length)
                    {
                        //文件存在，并且md5一样，不用上传
                        if (await SysUtility.CalculateMD5Async(uploadToPath) == fileinfo.MD5)
                        {
                            fileInfos.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }

                    //查看其他程序，看看有没有相同的文件
                    foreach (var otherPro in otherProjects)
                    {
                        var otherPath = Path.Combine(Global.AppConfig.Current.PublishRootPath, otherPro.Name);
                        if (!string.IsNullOrWhiteSpace(otherPro.PublishPath))
                        {
                            otherPath = otherPro.PublishPath;
                        }
                        otherPath = Path.Combine(otherPath, fileinfo.Path);

                        if (System.IO.File.Exists(otherPath) && new FileInfo(otherPath).Length == fileinfo.Length)
                        {
                            //文件存在，并且md5一样，不用上传
                            if (await SysUtility.CalculateMD5Async(otherPath) == fileinfo.MD5)
                            {
                                //拷贝文件
                                System.IO.File.Copy(otherPath, uploadToPath, true);

                                fileInfos.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                    }
                }

                if (project.FileHasUpdated == false)
                {
                    if (fileInfos.Count > 0)
                    {
                        project.FileHasUpdated = true;
                    }
                }
                await _db.UpdateAsync(project);
                return fileInfos.Select(m => m.Path).ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                throw;
            }
        }

        [HttpPost]
        public async Task Upload([FromForm] string projectGuid, [FromForm] string path, [FromForm] string md5, [FromForm] long length, [FromForm] DateTime writeTimeUtc)
        {
            var project = await _db.Project.FirstOrDefaultAsync(m => m.Guid == projectGuid);
            if (project == null)
                throw new ServiceException("此程序服务不存在");

            var publishPath = Path.Combine(Global.AppConfig.Current.PublishRootPath, project.Name);
            if (!string.IsNullOrWhiteSpace(project.PublishPath))
            {
                publishPath = project.PublishPath;
            }

            var uploadToPath = Path.Combine(publishPath, "$$$publish_agent_update", path);

            var file = Request.Form.Files[0];
            using Stream stream = file.OpenReadStream();
            if (System.IO.File.Exists(uploadToPath))
            {
                try
                {
                    SysUtility.DeleteFile(uploadToPath);
                }
                catch (Exception ex)
                {
                    throw new Exception($"删除文件失败，{uploadToPath},{ex.Message}");
                }
            }
            using (var target = System.IO.File.Create(uploadToPath))
            {
                SysUtility.Decompress(stream, target);
            }
            new FileInfo(uploadToPath).LastWriteTimeUtc = writeTimeUtc;


        }


        /// <summary>
        /// 手动停止项目
        /// </summary>
        /// <param name="guid"></param>
        [HttpGet]
        public async Task StopProject(string guid)
        {
            var project = await _db.Project.FirstOrDefaultAsync(m => m.Guid == guid);


            var item = _keepProcessAliveFactory.Create(guid);
            if (item != null)
            {
                await item.Stop();
            }

            project.IsStopped = true;
            await _db.UpdateAsync(project);
        }

        [HttpGet]
        public async Task StopBuild(string guid)
        {
            if(_projectTtyWorker.TryGetValue(guid , out TtyWorker worker))
            {
                await worker.Kill();
            }
        }

        /// <summary>
        /// 手动启动项目
        /// </summary>
        /// <param name="guid"></param>
        [HttpGet]
        public async Task StartProject(string guid)
        {
            var project = await _db.Project.FirstOrDefaultAsync(m => m.Guid == guid);
            if (!string.IsNullOrWhiteSpace(project.RunCmd) || project.RunType != Project_RunTypeEnum.Program)
            {
                var item = _keepProcessAliveFactory.Create(guid);
                if (item != null)
                {
                    await item.Start();
                }
            }
        }

        [HttpGet]
        public async Task<IEnumerable<RunningInfo>> GetAllProjectRunningInfos()
        {
            var runningInfoProviders = Global.ServiceProvider.GetServices<IRunningInfoProvider>();
            var allprojects = await _db.Project.ToArrayAsync();
            var groupProjects = from m in allprojects
                                group m by m.RunType into g
                                select new
                                {
                                    RunnerType = g.Key,
                                    Projects = g.ToArray()
                                };

            List<RunningInfo> outputInfos = new List<RunningInfo>();
            foreach (var group in groupProjects)
            {
                var provider = runningInfoProviders.FirstOrDefault(m => m.RunnerType == group.RunnerType);
                if (provider != null)
                {
                    var infos = await provider.GetCpuUsagePercent(group.Projects);
                    outputInfos.AddRange(infos);
                }
            }

            return outputInfos;
        }

        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> GetConfigContent(string guid, string filename)
        {
            var project = await _db.Project.FirstOrDefaultAsync(m => m.Guid == guid);
            var publishPath = Path.Combine(Global.AppConfig.Current.PublishRootPath, project.Name);
            if (!string.IsNullOrWhiteSpace(project.PublishPath))
            {
                publishPath = project.PublishPath;
            }
            filename = Path.Combine(publishPath, filename);

            if (System.IO.File.Exists(filename) == false)
                throw new ServiceException($"没有找到文件：{filename}");
            return await System.IO.File.ReadAllTextAsync(filename, Encoding.UTF8);
        }

        [HttpGet]
        public async Task<string[]> GetTextFiles(string guid)
        {
            var project = await _db.Project.FirstOrDefaultAsync(m => m.Guid == guid);
            var publishPath = Path.Combine(Global.AppConfig.Current.PublishRootPath, project.Name);
            if (!string.IsNullOrWhiteSpace(project.PublishPath))
            {
                publishPath = project.PublishPath;
            }
            if (Directory.Exists(publishPath) == false)
                return new string[0];

            var files = Directory.EnumerateFiles(publishPath);
            string[] exts = new string[] { ".json", ".txt", ".xml" };
            return files.Where(m => exts.Contains(Path.GetExtension(m))).Select(m => Path.GetFileName(m)).OrderBy(m => m).ToArray();
        }

        [HttpPost]
        public async Task SaveConfigContent([FromForm] string guid, [FromForm] string filename, [FromForm] string content)
        {
            var project = await _db.Project.FirstOrDefaultAsync(m => m.Guid == guid);
            var publishPath = Path.Combine(Global.AppConfig.Current.PublishRootPath, project.Name);
            if (!string.IsNullOrWhiteSpace(project.PublishPath))
            {
                publishPath = project.PublishPath;
            }
            filename = Path.Combine(publishPath, filename);

            var dirPath = Path.GetDirectoryName(filename);
            if (Directory.Exists(dirPath) == false){
                Directory.CreateDirectory(dirPath);
            }

            await System.IO.File.WriteAllTextAsync(filename, content, Encoding.UTF8);

            project.FileHasUpdated = true;
            await _db.UpdateAsync(project);

        }



        [HttpGet]
        public async Task<string> GetDockerfile(string guid)
        {
            var project = await _db.Project.FirstOrDefaultAsync(m => m.Guid == guid);
            var filepath = Path.Combine("./Dockerfiles", project.Guid);
            return await System.IO.File.ReadAllTextAsync(filepath, Encoding.UTF8);
        }



        [HttpPost]
        public async Task SaveDockerfile([FromForm] string guid, [FromForm] string content)
        {
            var project = await _db.Project.FirstOrDefaultAsync(m => m.Guid == guid);
            if (Directory.Exists("./Dockerfiles") == false)
            {
                Directory.CreateDirectory("./Dockerfiles");
            }
            var filepath = Path.Combine("./Dockerfiles", project.Guid);
            project.FileHasUpdated = true;
            await _db.UpdateAsync(project);

            _keepProcessAliveFactory.UpdateProject(project);

            await System.IO.File.WriteAllTextAsync(filepath, content, Encoding.UTF8);
        }

        [HttpGet]
        public async Task<IEnumerable<BackupFileResponse>> GetProjectBackups(string guid)
        {
            var backupFolder = Path.Combine(Global.AppConfig.Current.BackupPath, guid);
            if (Directory.Exists(backupFolder) == false)
            {
                return new BackupFileResponse[0];
            }
            var subDirs = Directory.GetDirectories(backupFolder);
            BackupFileResponse[] ret = new BackupFileResponse[subDirs.Length];
            for (int i = 0; i < subDirs.Length; i++)
            {
                try
                {
                    var folderName = Path.GetFileName(subDirs[i]);
                    var item = ret[i] = new BackupFileResponse();
                    item.CreateTime = long.Parse(folderName);
                    item.FileName = folderName;
                }
                catch
                {

                }

            }

            return ret.OrderByDescending(m => m.CreateTime);
        }

    }
}