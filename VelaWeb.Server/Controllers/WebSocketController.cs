using Microsoft.AspNetCore.Mvc;
using VelaLib;
using VelaWeb.Server.DBModels;
using VelaWeb.Server.Infrastructures;
using VelaWeb.Server.Models;
using System.Collections.Concurrent;
using System.Net.Http;
using Way.Lib;

namespace VelaWeb.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebSocketController : ControllerBase
    {
        private readonly WebSocketConnectionCenter _webSocketConnectionCenter;

        public WebSocketController(WebSocketConnectionCenter webSocketConnectionCenter)
        {
            _webSocketConnectionCenter = webSocketConnectionCenter;
           
        }

        

        [HttpGet]
        public async Task Get()
        {
            if(Request.HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var websocket = await Request.HttpContext.WebSockets.AcceptWebSocketAsync();
                _webSocketConnectionCenter.AddConnection(websocket);
                try
                {
                    while (true)
                    {
                        using var cancellationTokenSource = new CancellationTokenSource(3000);
                        var ret = await websocket.ReadString(cancellationTokenSource.Token);
                    }
                }
                catch
                {

                }
                _webSocketConnectionCenter.RemoveConnection(websocket);
            }
        }
    }
}
