using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.WebSockets;
using System.Text;
using VelaAgent.DBModels;
using VelaAgent.Infrastructures;
using VelaLib;
using Way.Lib;

namespace VelaAgent.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UpgradeController:ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<UpgradeController> _logger;

        public UpgradeController(IFileService fileService,ILogger<UpgradeController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        [HttpPost]
        public void UploadZip([FromBody]object state)
        {
            string filepath = this.Request.Headers["FilePath"].ToString();
            if (System.IO.Path.GetExtension(this.Request.Headers["Name"].ToString()) != ".zip")
            {
                SysUtility.DeleteFile(filepath);
                throw new ServiceException("只能上传.zip文件");
            }

            _logger.LogInformation("收到更新包");
            System.IO.File.Move(filepath, "./Upgrade.zip", true);

            var model = new ServiceUpgradeConfigModel() { 
                Zip = "Upgrade.zip",
                ExcludeFiles = new string[] { "appsettings.json" }
            };
            System.IO.File.WriteAllText("./VelaService.upgrade.json", model.ToJsonString(), Encoding.UTF8);
            
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                Process.GetCurrentProcess().Kill();
            });
        }
    }
    public class ServiceUpgradeConfigModel
    {
        public string Zip { get; set; }
        /// <summary>
        /// 排除的文件
        /// </summary>
        public string[] ExcludeFiles { get; set; }
    }
}
