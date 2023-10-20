using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using VelaWeb.Dtos;
using VelaWeb.Server.DBModels;
using VelaWeb.Dtos;
using Way.Lib;
using JMS.Token;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Xml.Linq;
using EJ;
using static System.Runtime.InteropServices.JavaScript.JSType;
using VelaLib;
using VelaWeb.Server.Infrastructures;
using System.IO.Compression;

namespace VelaWeb.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class UpgradeController : AuthController
    {
        private readonly SysDBContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUpgradePackageService _upgradePackageService;

        public UpgradeController(SysDBContext db, IHttpClientFactory httpClientFactory, IUpgradePackageService upgradePackageService)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _upgradePackageService = upgradePackageService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<string> UploadAgent([FromBody] object state)
        {
            string filepath = this.Request.Headers["FilePath"].ToString();
            if (System.IO.Path.GetExtension(this.Request.Headers["Name"].ToString()) != ".zip")
            {
                SysUtility.DeleteFile(filepath);
                throw new ServiceException("只能上传.zip文件");
            }


            if (_upgradePackageService.CheckPackage(filepath))
            {
                System.IO.File.Move(filepath, Global.AgentUpgradeFilePath, true);
                return _upgradePackageService.GetVersion(false);
            }
            else
            {
                if (_upgradePackageService.IsWebServerPackage(filepath))
                {
                    _upgradePackageService.UpgradeWebServer(filepath);
                    return "upgradeWebServer";
                }

                SysUtility.DeleteFile(filepath);
                throw new ServiceException("无效的更新包");
            }
        }

        [HttpGet]
        public string GetUpgradeVersion()
        {
            return _upgradePackageService.GetVersion();
        }

        [AllowAnonymous]//这是websocket连接，无法进行身份验证
        [HttpGet]
        public async Task UpgradeAgent(int agentId)
        {
            if (System.IO.File.Exists(Global.AgentUpgradeFilePath) == false)
                throw new ServiceException("未上传Agent更新包");

            if (Request.HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var websocket = await Request.HttpContext.WebSockets.AcceptWebSocketAsync();
                try
                {
                    var agent = await _db.Agent.FirstOrDefaultAsync(m => m.id == agentId);
                    var client = Global.ServiceProvider.GetRequiredService<JMS.JmsUploadClient>();
                    client.UploadProgress += async (s, percent) =>
                    {
                        await websocket.SendString($"正在传输更新包 {percent}%...");
                    };
                    await client.Upload($"https://{agent.Address}:{agent.Port}/Upgrade/UploadZip", Global.AgentUpgradeFilePath, null, new { });
                    await websocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "ok", CancellationToken.None);
                }
                catch (Exception ex)
                {
                    await websocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.InternalServerError, ex.Message, CancellationToken.None);
                }
            }


        }
    }
}
