using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pomelo.Data.MySql.Memcached;
using VelaLib;
using VelaWeb.Server.DBModels;
using VelaWeb.Server.Models;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Web;
using Way.Lib;
using static System.Runtime.InteropServices.JavaScript.JSType;
using VelaWeb.Server.Infrastructures;
using VelaWeb.Server.Dtos;
using VelaAgent.DBModels.Dtos;
using System.Data;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Diagnostics;
using VelaWeb.Server.Git;

namespace VelaWeb.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class AgentServiceController : AuthController
    {
        private readonly SysDBContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AgentsManager _agentsManager;
        private readonly WebSocketConnectionCenter _webSocketConnectionCenter;
        private readonly GitManager _gitManager;
        private readonly IProjectBuildInfoOutput _projectBuildInfoOutput;
        private readonly BuildingManager _buildingManager;
        private readonly IGitService _gitService;
        private readonly ProjectCenter _projectCenter;
        private readonly ILogger<AgentServiceController> _logger;
        static ConcurrentDictionary<string, bool> _publishingDict = new ConcurrentDictionary<string, bool>();

        public AgentServiceController(SysDBContext db, IHttpClientFactory httpClientFactory,
            AgentsManager agentsManager, 
            WebSocketConnectionCenter webSocketConnectionCenter,
            GitManager gitManager, IProjectBuildInfoOutput projectBuildInfoOutput,
            BuildingManager buildingManager,
            IGitService gitService, ProjectCenter projectCenter, ILogger<AgentServiceController> logger)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _agentsManager = agentsManager;
            _webSocketConnectionCenter = webSocketConnectionCenter;
            _gitManager = gitManager;
            _projectBuildInfoOutput = projectBuildInfoOutput;
            _buildingManager = buildingManager;
            _gitService = gitService;
            _projectCenter = projectCenter;
            _logger = logger;
        }

        [HttpGet]
        public Task<Agent[]> GetAgents()
        {
            return (from m in _db.Agent
                    orderby m.Address, m.Port
                    select m).ToArrayAsync();
        }

        [HttpGet]
        public string[] GetBuildingProjects()
        {
            return _buildingManager.GetBuildingProjects();
        }

        [HttpGet]
        public async Task<string> GetAgentVersion(long id)
        {
            var agent = await _db.Agent.FirstOrDefaultAsync(m => m.id == id);
            await checkAgentState(agent);
            var client = _httpClientFactory.CreateClient("");
            var ret = await client.GetAsync($"https://{agent.Address}:{agent.Port}/Publish/GetVersion");
            var txtRet = await ret.Content.ReadAsStringAsync();
            if (ret.IsSuccessStatusCode == false)
            {
                throw new ServiceException(txtRet);
            }
            return txtRet;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<long> AddAgent([FromBody] Agent agent)
        {
            await checkAgentState(agent);

            if (await _db.Agent.AnyAsync(m => m.Address == agent.Address && m.Port == agent.Port))
            {
                throw new ServiceException($"服务器已存在");
            }

            if (string.IsNullOrWhiteSpace(agent.Category))
            {
                throw new ServiceException($"请填写类别");
            }

            _db.BeginTransaction();
            await _db.InsertAsync(agent);

            await _db.InsertAsync(new Logs
            {
                UserId = this.UserId,
                Operation = "添加服务器",
                Time = DateTime.UtcNow,
                Detail = agent.ToJsonString()
            });

            _db.CommitTransaction();

            _agentsManager.OnNewAgent(agent);


            return agent.id.Value;
        }

        async Task checkAgentState(Agent agent)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("");
                var ret = await client.GetStringAsync($"https://{agent.Address}:{agent.Port}/Publish/HasSetClientCert");
                if (Convert.ToBoolean(ret) == false)
                {
                    await client.GetStringAsync($"https://{agent.Address}:{agent.Port}/Publish/SetClientCert?certHash=" + Global.ClientCertHash);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                if (ex is SocketException se && se.ErrorCode == 10053)
                {
                    throw new ServiceException($"访问服务器失败，服务器可能已经绑定了其他客户端证书，如果要和你的客户端重新绑定，需要清除Publish Agent根目录下的data.ClientCertHash文件，并重启Publish Agent。");
                }
                else
                {
                    throw new ServiceException($"访问服务器失败，{ex.Message}，可能是服务器已经绑定了其他客户端证书，如果要和你的客户端重新绑定，需要清除Publish Agent根目录下的data.ClientCertHash文件，并重启Publish Agent。");
                }
            }
        }

        [HttpPost]
        public string GetPublishPath([FromBody] ProjectModel project)
        {
            return project.GetPublishPath();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task ModifyAgent([FromBody] Agent agent)
        {
            await checkAgentState(agent);
            if (await _db.Agent.AnyAsync(m => m.id != agent.id && m.Address == agent.Address && m.Port == agent.Port))
            {
                throw new ServiceException($"服务器已存在");
            }
            if (string.IsNullOrWhiteSpace(agent.Category))
            {
                throw new ServiceException($"请填写类别");
            }
            _db.BeginTransaction();
            var data = new Agent();
            agent.CopyValueTo(data, true, true);
            await _db.UpdateAsync(data);

            await _db.InsertAsync(new Logs
            {
                UserId = this.UserId,
                Operation = "修改服务器",
                Time = DateTime.UtcNow,
                Detail = agent.ToJsonString()
            });

            _db.CommitTransaction();

            _projectCenter.OnAgentUpdate(agent);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task DeleteAgent(long id)
        {
            _db.BeginTransaction();
            await _db.DeleteAsync<Agent>(x => x.id == id);

            await _db.InsertAsync(new Logs
            {
                UserId = this.UserId,
                Operation = "删除服务器",
                Time = DateTime.UtcNow,
                Detail = id.ToString()
            });

            _db.CommitTransaction();

            _agentsManager.OnRemoveAgent(id);
        }

        /// <summary>
        /// 获取所有部署的程序
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ProjectModel>> GetProjects(string? search)
        {
            var allProjects = _projectCenter.GetAllProjects();
            if (!string.IsNullOrWhiteSpace(search))
            {
                allProjects = allProjects.Where(m => m.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                || m.Desc?.Contains(search, StringComparison.OrdinalIgnoreCase) == true
                || m.BuildCmd?.Contains(search, StringComparison.OrdinalIgnoreCase) == true).ToArray();
            }
            var list = allProjects.ToJsonString().FromJson<ProjectModel[]>();
            if (this.User.FindFirstValue(ClaimTypes.Role) != "Admin")
            {
                //检查权限可以列出什么工程
                var guids = await _db.UserProjectPower.Where(m => m.UserId == this.UserId).Select(m => m.ProjectGuid).ToArrayAsync();
                list = (from m in list
                        where m.UserId == this.UserId || guids.Contains(m.Guid)
                        select m).ToArray();
            }

            var userids = list.Select(m => m.UserId).ToArray();
            var userinfos = await _db.UserInfo.Where(m => userids.Contains(m.id)).Select(x => new
            {
                name = x.Name,
                id = x.id
            }).ToArrayAsync();

            for (int i = 0; i < userids.Length; i++)
            {
                //清除git密码，以免泄露
                list[i].GitPwd = "";
                if (userids[i] != null)
                {
                    list[i].User = userinfos.FirstOrDefault(m => m.id == userids[i])?.name;
                }
            }
            return list.OrderBy(m => m.OwnerServer.Category).ThenBy(m => m.Name);
        }

        [HttpGet]
        public string GetGitPwd(string guid)
        {
            var project = _projectCenter.GetAllProjects().FirstOrDefault(m => m.Guid == guid);
            var role = this.User.FindFirstValue(ClaimTypes.Role);
            if (role == "Admin" || project.UserId == this.UserId || _db.UserProjectPower.Any(m => m.UserId == this.UserId && m.ProjectGuid == guid))
            {
                return Way.Lib.AES.Decrypt(project.GitPwd, Global.SecretKey);
            }
            throw new ServiceException("您没有此程序的操作权限");
        }

        async Task loadServiceListByServer(Agent agentItem, ConcurrentDictionary<ProjectModel, bool> list)
        {
            AgentModel agentModel = agentItem.ToJsonString().FromJson<AgentModel>();
            var client = _httpClientFactory.CreateClient("");
            try
            {
                var ret = await client.GetStringAsync($"https://{agentItem.Address}:{agentItem.Port}/Publish/GetAllProjects");
                var projects = ret.FromJson<ProjectModel[]>();
                foreach (var p in projects)
                {
                    p.OwnerServer = agentModel;
                    list[p] = true;
                }
            }
            catch (Exception ex)
            {

            }
        }

        [HttpPost]
        public async Task<string[]> GetBranchNames([FromBody] ProjectModel projectModel)
        {
            if (!string.IsNullOrWhiteSpace(projectModel.GitUrl))
            {

                return await _gitService.ListBranchesAsync(projectModel.GitUrl.Trim(), projectModel.GitUserName, projectModel.GitPwd);


            }
            return new string[0];
        }

        /// <summary>
        /// 新增发布程序
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<string> AddProject([FromBody] ProjectModel project)
        {
            project.GitUrl = project.GitUrl?.Trim();
            project.GitUserName = project.GitUserName?.Trim();
            project.GitRemote = project.GitRemote?.Trim();

            //if (string.IsNullOrEmpty(project.GitUrl) || string.IsNullOrEmpty(project.BranchName) || string.IsNullOrEmpty(project.GitRemote))
            //{
            //    throw new ServiceException("git信息填写不完整");
            //}

            var role = this.User.FindFirstValue(ClaimTypes.Role);

            if (role != "Admin")
            {
                if (await _db.UserAgentPower.AnyAsync(m => m.UserId == this.UserId && m.AgentId == project.OwnerServer.id) == false)
                {
                    throw new ServiceException($"您无权进行此项操作");
                }
            }

            var client = _httpClientFactory.CreateClient("");
            project.GitPwd = Way.Lib.AES.Encrypt(project.GitPwd, Global.SecretKey);
            project.UserId = this.UserId;
            project.ProgramPath = project.ProgramPath?.Trim();
            project.BuildCmd = project.BuildCmd?.Trim();
            project.RunCmd = project.RunCmd?.Trim();
            project.BuildPath = project.BuildPath?.Trim();

            HttpContent content = new StringContent(project.ToJsonString());
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            using var response = await client.PostAsync($"https://{project.OwnerServer.Address}:{project.OwnerServer.Port}/Publish/AddProject", content);//改成自己的
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var msg = await response.Content.ReadAsStringAsync();
                throw new Exception(msg);
            }

            project.Guid = await response.Content.ReadAsStringAsync();
            _projectCenter.OnProjectUpdate(project);

            await _db.InsertAsync(new Logs
            {
                UserId = this.UserId,
                Operation = "新增发布程序",
                Time = DateTime.UtcNow,
                Detail = project.Guid + "\r\n" + project.Name + " " + project.GitUrl
            });


            return project.Guid;
        }

        [HttpPost]
        public async Task ModifyProject([FromBody] ProjectModel project)
        {
            project.GitUrl = project.GitUrl?.Trim();
            project.GitUserName = project.GitUserName?.Trim();
            project.GitRemote = project.GitRemote?.Trim();

            //if (string.IsNullOrEmpty(project.GitUrl) || string.IsNullOrEmpty(project.BranchName) || string.IsNullOrEmpty(project.GitRemote))
            //{
            //    throw new ServiceException("git信息填写不完整");
            //}

            checkProjectForUserRole(project.Guid);

            var client = _httpClientFactory.CreateClient("");
            project.GitPwd = Way.Lib.AES.Encrypt(project.GitPwd, Global.SecretKey);


            HttpContent content = new StringContent(project.ToJsonString());
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            using var response = await client.PostAsync($"https://{project.OwnerServer.Address}:{project.OwnerServer.Port}/Publish/ModifyProject", content);//改成自己的
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var msg = await response.Content.ReadAsStringAsync();
                throw new Exception(msg);
            }

            _projectCenter.OnProjectUpdate(project);
            _projectCenter.RefreshProject(project);

            await _db.InsertAsync(new Logs
            {
                UserId = this.UserId,
                Operation = "修改发布程序",
                Time = DateTime.UtcNow,
                Detail = project.Guid + "\r\n" + project.Name + " " + project.GitUrl
            });

            project.GitPwd = "";
            await _webSocketConnectionCenter.SendTextAsync($"Modify:{project.ToJsonString()}");
        }

        [HttpPost]
        public async Task DeleteProject([FromBody] ProjectModel project)
        {
            var dbpro = checkProjectForUserRole(project.Guid);

            var role = this.User.FindFirstValue(ClaimTypes.Role);
            if (role == "Admin" || dbpro.UserId == this.UserId)
            {

            }
            else
            {
                throw new ServiceException("您没有删除权限");
            }
            var client = _httpClientFactory.CreateClient("");
            await client.GetStringAsync($"https://{project.OwnerServer.Address}:{project.OwnerServer.Port}/Publish/DeleteProject?guid={project.Guid}");

            _projectCenter.OnProjectDelete(project);

            await _db.InsertAsync(new Logs
            {
                UserId = this.UserId,
                Operation = "删除发布程序",
                Time = DateTime.UtcNow,
                Detail = project.Guid + "\r\n" + project.Name
            });

            await _webSocketConnectionCenter.SendTextAsync($"Delete:{project.Guid}");
        }

        ProjectModel checkProjectForUserRole(string guid)
        {
            var project = _projectCenter.GetAllProjects().FirstOrDefault(m => m.Guid == guid);
            var role = this.User.FindFirstValue(ClaimTypes.Role);
            if (role == "Admin" || project.UserId == this.UserId || _db.UserProjectPower.Any(m => m.UserId == this.UserId && m.ProjectGuid == guid))
            {

            }
            else
            {
                throw new ServiceException("您没有此程序的操作权限");
            }
            return project;
        }

        /// <summary>
        /// 重启本服务
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public void Restart()
        {
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// 重启agent服务
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task RestartAgent(AgentModel agentModel)
        {
            var client = _httpClientFactory.CreateClient("");
            var response = await client.GetAsync($"https://{agentModel.Address}:{agentModel.Port}/Publish/Restart");
            var ret = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new ServiceException(ret);
        }

        [HttpGet]
        public async Task SetTermResize(string guid, int cols, int rows)
        {
            
        }

        async Task login(string username, string pwd)
        {
            pwd = Way.Lib.AES.Encrypt(pwd, Global.SecretKey);
            var userinfo = await _db.UserInfo.FirstOrDefaultAsync(m => m.Name == username);
            if (userinfo == null)
                throw new ServiceException("用户名、密码错误");

            if (userinfo.IsLock)
                throw new ServiceException("账号已被锁定");

            if (userinfo.Password != pwd)
            {
                userinfo.ErrorCount++;
                if (userinfo.ErrorCount >= 10)
                {
                    userinfo.IsLock = true;
                }
                await _db.UpdateAsync(userinfo);
                throw new ServiceException("用户名、密码错误");
            }

            userinfo.ErrorCount = 0;
            await _db.UpdateAsync(userinfo);

            var claimsIdentity = new ClaimsIdentity(new Claim[]
        {
                new Claim(ClaimTypes.NameIdentifier, userinfo.id.ToString()),
                new Claim(ClaimTypes.Role , userinfo.Role.ToString()),
        }, "JMS.Token"); ;

            this.User.AddIdentity(claimsIdentity);
        }

        /// <summary>
        /// 手动运行项目，先拉取git，再编译运行
        /// </summary>
        /// <param name="guid"></param>
        [AllowAnonymous]
        [HttpGet]
        public async Task RunWithUser(string guid, string username, string pwd)
        {
            await login(username, pwd);
            checkProjectForUserRole(guid);

            var project = _projectCenter.GetProject(guid);
            var worker = _gitManager.GetWorker(project);
            if (worker == null || worker.Ready == false)
            {
                throw new ServiceException("git仓库未就绪");
            }

            if (_publishingDict.ContainsKey(guid))
                throw new ServiceException("程序已经在发布中...");
            publish(worker, project, guid, 0, 0);
        }

        /// <summary>
        /// 手动运行项目，先拉取git，再编译运行
        /// </summary>
        /// <param name="guid"></param>
        [HttpGet]
        public void Run(string guid, int cols, int rows)
        {
            checkProjectForUserRole(guid);

            var project = _projectCenter.GetProject(guid);
            var worker = _gitManager.GetWorker(project);
            if (worker == null || worker.Ready == false)
            {
                throw new ServiceException("git仓库未就绪");
            }
            if (_publishingDict.ContainsKey(guid))
                throw new ServiceException("程序已经在发布中...");
            publish(worker, project, guid, cols, rows);
        }


        async void publish(GitWorker worker, ProjectModel project, string guid, int cols, int rows)
        {
            if (_publishingDict.TryAdd(guid, true))
            {
                try
                {
                    await _projectCenter.PullAndBuildAsync(worker, cols, rows, guid, project.GetGitHash(), false, null);
                }
                catch (Exception)
                {

                }
                finally
                {
                    _publishingDict.TryRemove(guid, out _);
                }
            }
        }

        [HttpGet]
        public void StopBuild(string guid)
        {
            _projectCenter.StopBuild(guid);
        }


        [HttpGet]
        public async Task<FileContentResult> GetOutputInfo(string guid)
        {
            var data = await _projectBuildInfoOutput.Flush(guid);
            if (data == null)
            {
                data = new byte[0];
            }

            return File(data, "application/octet-stream");
        }

        [HttpGet]
        public async Task<string[]> GetTextFiles(string guid)
        {
            var project = _projectCenter.GetProject(guid);
            var client = _httpClientFactory.CreateClient("");
            var response = await client.GetAsync($"https://{project.OwnerServer.Address}:{project.OwnerServer.Port}/Publish/GetTextFiles?guid={project.Guid}");
            var ret = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new ServiceException(ret);
            return ret.FromJson<string[]>();
        }


        [HttpGet]
        public LogFile[] GetLogFiles(string guid)
        {
            var project = _projectCenter.GetProject(guid);
            var folderHash = project.GetGitHash();
            var logFolder = $"./Logs/{folderHash}/{guid}";
            if (Directory.Exists(logFolder) == false)
                return new LogFile[0];

            var files = Directory.GetFiles(logFolder, "*.tty");
            List<LogFile> list = new List<LogFile>();
            foreach (var file in files)
            {
                var info = new FileInfo(file);
                var time = info.CreationTimeUtc;
                list.Add(new LogFile
                {
                    CreateTime = time,
                    FileName = Path.GetFileName(file),
                    Length = info.Length
                });
            }

            return list.OrderByDescending(m => m.CreateTime).ToArray();
        }

        [HttpPost]
        public async Task<FileContentResult> GetLogFileContent([FromForm] string guid, [FromForm] string filename)
        {
            var project = _projectCenter.GetProject(guid);
            var folderHash = project.GetGitHash();
            var logFolder = $"./Logs/{folderHash}/{guid}";
            using var fs = new FileStream(Path.Combine(logFolder, filename), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var data = new byte[fs.Length];
            await fs.ReadAtLeastAsync(data, data.Length);
            return File(data, "application/octet-stream");
        }

        /// <summary>
        /// 手动启动项目（无需编译）
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task StartProject(string guid)
        {
            checkProjectForUserRole(guid);
            var project = _projectCenter.GetProject(guid);
            var client = _httpClientFactory.CreateClient("");
            var ret = await client.GetAsync($"https://{project.OwnerServer.Address}:{project.OwnerServer.Port}/Publish/StartProject?guid={guid}");
            if (ret.IsSuccessStatusCode == false)
            {
                var msg = await ret.Content.ReadAsStringAsync();
                throw new ServiceException(msg);
            }
            await _db.InsertAsync(new Logs
            {
                UserId = this.UserId,
                Operation = "启动程序",
                Time = DateTime.UtcNow,
                Detail = project.Guid + "\r\n" + project.Name
            });
        }

        /// <summary>
        /// 重新克隆git
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        [HttpGet]
        public async Task ReCloneProject(string guid)
        {
            checkProjectForUserRole(guid);

            var project = _projectCenter.GetProject(guid);
            var worker = _gitManager.GetWorker(project);
            if (worker == null || worker.Ready == false)
            {
                throw new ServiceException("git仓库未就绪");
            }
            worker.ReClone();
        }

        /// <summary>
        /// 手动停止项目
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task StopProject(string guid)
        {
            checkProjectForUserRole(guid);
            var project = _projectCenter.GetProject(guid);
            var client = _httpClientFactory.CreateClient("");
            await client.GetStringAsync($"https://{project.OwnerServer.Address}:{project.OwnerServer.Port}/Publish/StopProject?guid={guid}");
            await _db.InsertAsync(new Logs
            {
                UserId = this.UserId,
                Operation = "停止程序",
                Time = DateTime.UtcNow,
                Detail = project.Guid + "\r\n" + project.Name
            });
        }

        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> GetConfigContent(string guid, string filename)
        {
            var project = _projectCenter.GetProject(guid);
            var client = _httpClientFactory.CreateClient("");
            using var response = await client.GetAsync($"https://{project.OwnerServer.Address}:{project.OwnerServer.Port}/Publish/GetConfigContent?guid={guid}&filename={HttpUtility.UrlEncode(filename)}");
            if (response.IsSuccessStatusCode == false)
            {
                var msg = await response.Content.ReadAsStringAsync();
                throw new Exception(msg);
            }

            return await response.Content.ReadAsStringAsync();
        }

        [HttpPost]
        public async Task SaveConfigContent([FromForm] string guid, [FromForm] string filename, [FromForm] string content)
        {
            checkProjectForUserRole(guid);
            var project = _projectCenter.GetProject(guid);
            var client = _httpClientFactory.CreateClient("");
            using (var formContent = new MultipartFormDataContent())
            {
                formContent.Add(new StringContent(guid), "guid");
                formContent.Add(new StringContent(filename), "filename");
                formContent.Add(new StringContent(content), "content");

                // 发送POST请求
                using var response = await client.PostAsync($"https://{project.OwnerServer.Address}:{project.OwnerServer.Port}/Publish/SaveConfigContent", formContent);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    throw new Exception(msg);
                }
            }
            await _db.InsertAsync(new Logs
            {
                UserId = this.UserId,
                Operation = "修改配置文件",
                Time = DateTime.UtcNow,
                Detail = project.Name + "\r\n" + filename
            });
        }

        [HttpGet]
        public async Task<string> GetDockerfile(string guid)
        {
            var project = _projectCenter.GetProject(guid);
            var client = _httpClientFactory.CreateClient("");
            using var response = await client.GetAsync($"https://{project.OwnerServer.Address}:{project.OwnerServer.Port}/Publish/GetDockerfile?guid={guid}");
            if (response.IsSuccessStatusCode == false)
            {
                var msg = await response.Content.ReadAsStringAsync();
                throw new Exception(msg);
            }

            return await response.Content.ReadAsStringAsync();
        }

        [HttpPost]
        public async Task SaveDockerfile([FromForm] string guid, [FromForm] string content)
        {
            checkProjectForUserRole(guid);
            var project = _projectCenter.GetProject(guid);
            var client = _httpClientFactory.CreateClient("");
            using (var formContent = new MultipartFormDataContent())
            {
                formContent.Add(new StringContent(guid), "guid");
                formContent.Add(new StringContent(content), "content");

                // 发送POST请求
                using var response = await client.PostAsync($"https://{project.OwnerServer.Address}:{project.OwnerServer.Port}/Publish/SaveDockerfile", formContent);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    throw new Exception(msg);
                }
            }
        }

        /// <summary>
        /// 获取程序的备份列表
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        public async Task<BackupFileResponse[]> GetProjectBackups(string guid)
        {
            var project = _projectCenter.GetProject(guid);
            try
            {
                var client = _httpClientFactory.CreateClient("");
                using var response = await client.GetAsync($"https://{project.OwnerServer.Address}:{project.OwnerServer.Port}/Publish/GetProjectBackups?guid={guid}");
                var json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(json);
                }

                return json.FromJson<BackupFileResponse[]>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task Restore(string guid, string backupFileName)
        {
            var project = _projectCenter.GetProject(guid);

            await _db.InsertAsync(new Logs
            {
                UserId = this.UserId,
                Operation = "从备份恢复",
                Time = DateTime.UtcNow,
                Detail = project.Name + "\r\n" + Path.GetFileName( backupFileName)
            });

            await project.Restore(backupFileName);
        }

    }
}
