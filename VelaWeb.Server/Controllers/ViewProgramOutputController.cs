using Microsoft.AspNetCore.Mvc;
using VelaLib;
using VelaWeb.Server.DBModels;
using VelaWeb.Server.Infrastructures;
using VelaWeb.Server.Models;
using System.Collections.Concurrent;
using System.Net.Http;
using Way.Lib;
using System.Net.WebSockets;

namespace VelaWeb.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ViewProgramOutputController : ControllerBase
    {
        private readonly ProjectCenter _projectCenter;

        public ViewProgramOutputController(ProjectCenter projectCenter)
        {
            _projectCenter = projectCenter;
        }

        [HttpGet]
        public async Task Get(string guid,int preline)
        {
            if(Request.HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var websocket = await Request.HttpContext.WebSockets.AcceptWebSocketAsync();
                try
                {
                    var project = _projectCenter.GetProject(guid);
                    using ClientWebSocket logSocket = Global.ServiceProvider.GetRequiredService<ClientWebSocket>();
                    await logSocket.ConnectAsync(new Uri($"wss://{project.OwnerServer.Address}:{project.OwnerServer.Port}/ProgramOutput?guid={guid}&preline={preline}"), CancellationToken.None);

                    receiveOutput(logSocket, websocket);
                    while (true)
                    {
                        var ret = await websocket.ReadString();
                    }
                }
                catch
                {

                }
            }
        }

        async void receiveOutput(WebSocket webSocket,WebSocket outputWebSocket)
        {
            try
            {
                string info;
                while (true)
                {
                    info = await webSocket.ReadString();
                    await outputWebSocket.SendString(info);
                }
            }
            catch 
            {
                try
                {
                    if (outputWebSocket.State == WebSocketState.Open)
                    {
                        await outputWebSocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.InternalServerError, "program exit", CancellationToken.None);
                    }
                }
                catch
                {

                }
            }
        }
    }
}
