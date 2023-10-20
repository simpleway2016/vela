using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System.Net.WebSockets;
using VelaAgent.DBModels;
using VelaAgent.Infrastructures;
using VelaAgent.KeepAlive;
using VelaLib;

namespace VelaAgent.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestoreController:ControllerBase,IInfoOutput
    {
        private readonly KeepProcessAliveFactory _keepProcessAliveFactory;
        private readonly IFileService _fileService;
        private readonly ProjectBackup _projectBackup;
        private readonly ILogger<UploadCompletedController> _logger;
        private readonly ICmdRunner _cmdRunner;
        public int Cols { get; private set; }
        public int Rows { get; private set; }
        public RestoreController(KeepProcessAliveFactory keepProcessAliveFactory,
            IFileService fileService,ProjectBackup projectBackup,
            ILogger<UploadCompletedController> logger,ICmdRunner cmdRunner)
        {
            _keepProcessAliveFactory = keepProcessAliveFactory;
            _fileService = fileService;
            _projectBackup = projectBackup;
            _logger = logger;
            _cmdRunner = cmdRunner;
        }

        WebSocket _websocket;
        [HttpGet]
        public async Task Get()
        {
            if (Request.HttpContext.WebSockets.IsWebSocketRequest)
            {
                KeepProcessAlive keepAliveObj = null;
                _websocket = await Request.HttpContext.WebSockets.AcceptWebSocketAsync();

                try
                {
                    var projectGuid = await _websocket.ReadString();
                    var backupFileName = await _websocket.ReadString();
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

                    //还原
                    await _projectBackup.Restore(db,projectGuid, publishPath , backupFileName,this);
                    keepAliveObj.Project.FileHasUpdated = true;
                    await db.UpdateAsync(keepAliveObj.Project);

                    if (!string.IsNullOrEmpty(keepAliveObj.Project.RunCmd) || keepAliveObj.Project.RunType != Project_RunTypeEnum.Program)
                    {
                        try
                        {
                            if (keepAliveObj.Project.RunType == Project_RunTypeEnum.Program)
                            {
                                var runfilename = _cmdRunner.GetRunFileName(keepAliveObj.Project.RunCmd);
                                runfilename = Path.Combine(publishPath, runfilename);
                                if (runfilename.StartsWith("./"))
                                    runfilename = Path.GetFullPath(runfilename, AppDomain.CurrentDomain.BaseDirectory);

                                if (System.IO.File.Exists(runfilename))
                                {
                                    //_logger.LogDebug($"chmod +x \"{runfilename}\"");
                                    await _fileService.Chmod(runfilename, "+x");
                                    //_logger.LogDebug($"chmod +x 成功");
                                }
                            }
                        }
                        catch (ServiceException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "尝试设置chmod出错");
                        }

                        try
                        {
                            keepAliveObj.Project.IsStopped = false;
                            db.Update(keepAliveObj.Project);

                            await keepAliveObj.Start();
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

                    await _websocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null)
                        ex = ex.InnerException;
                    await Output(ex.Message);
                    await _websocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.InternalServerError, "", CancellationToken.None);
                }
                finally
                {
                    keepAliveObj.InfoOutput = null;
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

        public Task Output(byte[] data, int? length = null)
        {
            return Task.CompletedTask;
        }
    }
}
