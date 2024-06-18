using Microsoft.AspNetCore.Mvc;
using VelaLib;
using VelaWeb.Server.DBModels;
using VelaWeb.Server.Infrastructures;
using VelaWeb.Server.Models;
using System.Collections.Concurrent;
using System.Net.Http;
using Way.Lib;
using static System.Net.Mime.MediaTypeNames;

namespace VelaWeb.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebSocketController : ControllerBase
    {
        private readonly WebSocketConnectionCenter _webSocketConnectionCenter;
        private readonly ProjectCenter _projectCenter;

        public WebSocketController(WebSocketConnectionCenter webSocketConnectionCenter , ProjectCenter projectCenter)
        {
            _webSocketConnectionCenter = webSocketConnectionCenter;
            _projectCenter = projectCenter;
        }

        

        [HttpGet]
        public async Task Get()
        {
            if(Request.HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var websocket = await Request.HttpContext.WebSockets.AcceptWebSocketAsync();

                var allProjects = _projectCenter.GetAllProjects();
                foreach (var projectModel in allProjects) {
                    try
                    {
                        using var cancellationTokenSource = new CancellationTokenSource(2000);
                        await websocket.SendString(new
                        {
                            Guid = projectModel.Guid,
                            Status = projectModel.Status,
                            Error = projectModel.Error
                        }.ToJsonString(), cancellationTokenSource.Token);
                    }
                    catch
                    {
                    }
                   
                }

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
