using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using VelaAgent.Infrastructures;
using VelaAgent.KeepAlive;
using VelaAgent.ProgramOutput;
using VelaLib;

namespace VelaAgent.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProgramOutputController : ControllerBase, IInfoOutput
    {
        private readonly KeepProcessAliveFactory _keepProcessAliveFactory;
        private readonly ProgramOutputFactory _programOutputFactory;
        private readonly ILogger<ProgramOutputController> _logger;
        public int Cols { get; private set; }
        public int Rows { get; private set; }
        public ProgramOutputController(KeepProcessAliveFactory keepProcessAliveFactory,
            ProgramOutputFactory programOutputFactory,
            ILogger<ProgramOutputController> logger)
        {
            _keepProcessAliveFactory = keepProcessAliveFactory;
            _programOutputFactory = programOutputFactory;
            _logger = logger;
        }

        WebSocket _websocket;
        IProgramOutput _programOutput;
        [HttpGet]
        public async Task Get(string guid, int preline)
        {
            if (Request.HttpContext.WebSockets.IsWebSocketRequest)
            {
                _websocket = await Request.HttpContext.WebSockets.AcceptWebSocketAsync();

                try
                {
                    var keepAliveObj = _keepProcessAliveFactory.Create(guid);
                    if (keepAliveObj.Project == null)
                    {
                        _keepProcessAliveFactory.RemoveCache(guid);
                        throw new ServiceException("此程序服务不存在");
                    }
                    using var programOutput = _programOutputFactory.CreaateProgramOutput(keepAliveObj);
                    _programOutput = programOutput;
                    if (programOutput == null)
                    {
                        throw new Exception("无法获取程序输出源");
                    }

                    checkState();

                    await programOutput.StartOutput(keepAliveObj.Project , this, preline);

                    if (_websocket.State == WebSocketState.Open)
                    {
                        await _websocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.InternalServerError, "program exit", CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null)
                        ex = ex.InnerException;
                    await Output(ex.ToString());

                    if (_websocket.State == WebSocketState.Open)
                    {
                        await _websocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.InternalServerError, "", CancellationToken.None);
                    }
                }
                finally
                {
                    _websocket.Dispose();
                }
            }
        }

        async void checkState()
        {
            try
            {
                while (true)
                {
                    await _websocket.ReadString();
                }
            }
            catch  
            {
                _programOutput?.Dispose();
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
