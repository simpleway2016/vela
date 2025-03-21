using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System.Net.WebSockets;
using System.Text;
using VelaAgent.DBModels;
using VelaAgent.Infrastructures;
using VelaAgent.KeepAlive;
using VelaLib;
using Way.Lib;

namespace VelaAgent.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadCompletedController:ControllerBase,IInfoOutput
    {
        private readonly KeepProcessAliveFactory _keepProcessAliveFactory;
        private readonly IFileService _fileService;
        private readonly ProjectBackup _projectBackup;
        private readonly ILogger<UploadCompletedController> _logger;
        private readonly ICmdRunner _cmdRunner;

        public int Cols { get; private set; }
        public int Rows { get; private set; }

        public UploadCompletedController(KeepProcessAliveFactory keepProcessAliveFactory,
            IFileService fileService,ProjectBackup projectBackup,
            ILogger<UploadCompletedController> logger,ICmdRunner cmdRunner)
        {
            _keepProcessAliveFactory = keepProcessAliveFactory;
            _fileService = fileService;
            _projectBackup = projectBackup;
            _logger = logger;
            _cmdRunner = cmdRunner;
        }

        async Task DeleteNoUseFiles(string[] newFileList, string velaFileListPath , string publisPath)
        {
            string[] oldFileList = System.IO.File.ReadAllText(velaFileListPath, Encoding.UTF8).FromJson<string[]>();
            var toDelteFiles = oldFileList.Where(m => newFileList.Any(x => string.Equals(x, m, StringComparison.OrdinalIgnoreCase)) == false).ToArray();
            foreach (string file in toDelteFiles)
            {
                var path = Path.Combine(publisPath, file);
                if (System.IO.File.Exists(path))
                {
                    try
                    {
                        SysUtility.DeleteFile(path);
                        await this.Output($"删除无用文件 {path}");
                    }
                    catch (Exception ex)
                    {
                        await this.Output($"无法删除无用文件 {path}，{ex.Message}");
                    }
                }
            }
        }

        WebSocket _websocket;
        [HttpGet]
        public async Task Get(int cols,int rows)
        {
            if (Request.HttpContext.WebSockets.IsWebSocketRequest)
            {
                KeepProcessAlive keepAliveObj = null;
                _websocket = await Request.HttpContext.WebSockets.AcceptWebSocketAsync();
                this.Cols = cols;
                this.Rows = rows;
                try
                {                   
                    var fileListContent = await _websocket.ReadString();
                    var newFileList = fileListContent.FromJson<string[]>();
                    var projectGuid = await _websocket.ReadString();

                    using var db = new SysDBContext();

                    keepAliveObj = _keepProcessAliveFactory.Create(projectGuid);
                    if (keepAliveObj.Project == null)
                    {
                        _keepProcessAliveFactory.RemoveCache(projectGuid);
                        throw new ServiceException("此程序服务不存在");
                    }

                    var publishPath = Path.Combine(Global.AppConfig.Current.PublishRootPath, keepAliveObj.Project.Name);
                    if (!string.IsNullOrWhiteSpace(keepAliveObj.Project.PublishPath))
                    {
                        publishPath = keepAliveObj.Project.PublishPath;
                    }
                    keepAliveObj.InfoOutput = this;
                    await keepAliveObj.Stop();

                    if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,Global.AppConfig.Current.FileListFolder)) == false)
                    {
                        Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Global.AppConfig.Current.FileListFolder));
                    }

                    var velaFileListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Global.AppConfig.Current.FileListFolder, keepAliveObj.Project.Guid);
                    if (System.IO.File.Exists(velaFileListPath))
                    {
                        //删除被移除的程序文件
                        if (keepAliveObj.Project.DeleteNoUseFiles)
                        {
                            await DeleteNoUseFiles(newFileList, velaFileListPath, publishPath);
                        }
                        SysUtility.DeleteFile(velaFileListPath);
                    }

                    var uploadToPath = Path.Combine(publishPath, "$$$publish_agent_update");
                    if (Directory.Exists(uploadToPath))
                    {
                        SysUtility.CopyFolder(uploadToPath, publishPath);
                        SysUtility.DeleteFolder(uploadToPath);
                    }

                    System.IO.File.WriteAllText(velaFileListPath, fileListContent, Encoding.UTF8);
                   
                    if (db.Project.Where(m => m.Guid == projectGuid).Select(m => m.BackupCount).FirstOrDefault() != 0)
                    {
                        await this.Output("正在创建备份...");
                        await _projectBackup.Create(db, keepAliveObj.Project, velaFileListPath, newFileList);
                    }

                    string ret = "0";
                    if (!string.IsNullOrEmpty(keepAliveObj.Project.RunCmd) || keepAliveObj.Project.RunType != Project_RunTypeEnum.Program)
                    {
                        var workdir = publishPath;
                        if (workdir.StartsWith("./"))
                            workdir = Path.GetFullPath(workdir, AppDomain.CurrentDomain.BaseDirectory);

                        try
                        {
                            await this.Output(workdir);
                            await _fileService.ChmodAll(workdir, "u+x");
                            await this.Output($"chmod -R u+x * 成功");
                        }
                        catch (Exception ex)
                        {
                            await this.Output($"chmod -R u+x * 失败");
                            await this.Output(ex.Message);
                        }

                        //await this.Output($"IsFirstUpload:{keepAliveObj.Project.IsFirstUpload}  ConfigFiles:{keepAliveObj.Project.ConfigFiles}");
                        try
                        {
                            if (keepAliveObj.Project.IsFirstUpload == false || string.IsNullOrWhiteSpace(keepAliveObj.Project.ConfigFiles))
                            {
                                keepAliveObj.Project.IsFirstUpload = false;
                                keepAliveObj.Project.IsStopped = false;
                                db.Update(keepAliveObj.Project);

                                //第一次默认不运行，因为要等用户修改配置文件
                                await keepAliveObj.Start();
                                ret = "1";
                            }
                            else
                            {
                                keepAliveObj.Project.IsFirstUpload = false;
                                db.Update(keepAliveObj.Project);
                            }
                        }
                        catch (Exception ex)
                        {
                            _keepProcessAliveFactory.RemoveCache(projectGuid);
                            throw;
                        }
                    }
                    else
                    {
                        _keepProcessAliveFactory.RemoveCache(projectGuid);
                    }
                    await _websocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, ret, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    await Output(ex.GetMessage());
                    await _websocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.InternalServerError, "", CancellationToken.None);
                }
                finally
                {
                    if (keepAliveObj != null)
                    {
                        keepAliveObj.InfoOutput = null;
                    }
                    _websocket.Dispose();
                }
            }
        }

        public async Task Output(string text)
        {
            if (_websocket.State == WebSocketState.Open)
            {
                await _websocket.SendString(text);
            }
        }
        public async Task Output(byte[] data,int? length = null)
        {
            if (_websocket.State == WebSocketState.Open)
            {
                await _websocket.SendBytes(data, length);
            }
        }
    }
}
