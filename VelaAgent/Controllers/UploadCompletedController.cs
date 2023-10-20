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

                    await this.Output("正在创建备份...");
                    await _projectBackup.Create(db, keepAliveObj.Project , velaFileListPath, newFileList);

                    string ret = "0";
                    if (!string.IsNullOrEmpty(keepAliveObj.Project.RunCmd) || keepAliveObj.Project.RunType != Project_RunTypeEnum.Program)
                    {
                        try
                        {
                            if (keepAliveObj.Project.RunType == Project_RunTypeEnum.Program)
                            {
                                var cmd = keepAliveObj.Project.RunCmd.Trim();
                                if (cmd.StartsWith("nohup ") && cmd.Length > 6)
                                    cmd = cmd.Substring(6).Trim();

                                var runfilename = _cmdRunner.GetRunFileName(cmd);
                                runfilename = Path.Combine(publishPath, runfilename);
                                if (runfilename.StartsWith("./"))
                                    runfilename = Path.GetFullPath(runfilename, AppDomain.CurrentDomain.BaseDirectory);

                                if (System.IO.File.Exists(runfilename))
                                {
                                    await this.Output($"chmod +x \"{runfilename}\"");
                                    await _fileService.Chmod(runfilename, "+x");
                                    await this.Output($"chmod +x \"{runfilename}\" 成功");
                                }
                                else
                                {
                                    await this.Output($"没有找到{runfilename},chmod 忽略");
                                }
                            }
                        }
                        catch (ServiceException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            await this.Output($"尝试设置chmod出错，{ex.Message}");
                            _logger.LogError(ex, "尝试设置chmod出错");
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
