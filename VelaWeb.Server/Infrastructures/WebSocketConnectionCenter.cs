using VelaLib;
using VelaWeb.Server.DBModels;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.WebSockets;
using Org.BouncyCastle.Ocsp;
using Way.Lib;

namespace VelaWeb.Server.Infrastructures
{
    public class WebSocketConnectionCenter
    {
        ConcurrentDictionary<WebSocket, bool> _webSockets = new ConcurrentDictionary<WebSocket, bool>();
        private readonly ILogger<WebSocketConnectionCenter> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AgentsManager _agentsManager;
        ConcurrentDictionary<long, bool> _allAgentIds = new ConcurrentDictionary<long, bool>();

        public WebSocketConnectionCenter(ILogger<WebSocketConnectionCenter> logger, IHttpClientFactory httpClientFactory, AgentsManager agentsManager)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _agentsManager = agentsManager;
        }

        public void AddConnection(WebSocket webSocket)
        {
            _webSockets.TryAdd(webSocket, true);

            var agents = _agentsManager.GetAllAgents();
            foreach (var agent in agents)
            {
                if (agent.LastSendText != null)
                {
                    using var cancellationTokenSource = new CancellationTokenSource(2000);
                    webSocket.SendString(agent.LastSendText,cancellationTokenSource.Token);
                }
            }
        }


        public void RemoveConnection(WebSocket webSocket)
        {
            _webSockets.TryRemove(webSocket, out bool o);
        }

        /// <summary>
        /// 给所有连接发送信息
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task SendTextAsync(string text)
        {
            foreach (var pair in _webSockets)
            {
                try
                {
                    using var cancellationTokenSource = new CancellationTokenSource(2000);
                    await pair.Key.SendString(text , cancellationTokenSource.Token);
                }
                catch
                {
                }
            }
        }

        public async Task SendBinaryAsync(byte[] data, int? length = null)
        {
            foreach (var pair in _webSockets)
            {
                try
                {
                    using var cancellationTokenSource = new CancellationTokenSource(2000);
                    await pair.Key.SendBytes(data, cancellationTokenSource.Token, length);
                }
                catch
                {
                }
            }
        }
    }
}
