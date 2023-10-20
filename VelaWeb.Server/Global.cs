using VelaWeb.Server.DBModels;
using VelaLib;
using System.Security.Cryptography.X509Certificates;

namespace VelaWeb
{
    public class Global
    {
        public static string AppServiceName { get;set; }
        public static IServiceProvider ServiceProvider { get; private set; }
        public static string ClientCertHash { get; private set; }
        public static X509Certificate2 ClientCert { get; private set; }
        const string CertFilePath = "./clientcert.pfx";
        public const string SecretKey = "kijhgf9873@%&902";
        public const string ClientSecretKey = "dimh3765&@#l(ufg";
        public const string AgentUpgradeFilePath = "./AgentUpgrade.zip";

        public static int MaxRequestBuilding { get; private set; }

        public static void Init(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            var certificateService = ServiceProvider.GetRequiredService<ICertificateService>();
            MaxRequestBuilding = serviceProvider.GetRequiredService<IConfiguration>().GetSection("MaxRequestBuilding").Get<int?>().GetValueOrDefault();
            if (MaxRequestBuilding < 1)
                MaxRequestBuilding = 1;

            if (File.Exists(CertFilePath) == false)
            {
                certificateService.CreateCertificate("VelaLibDesktop", 100, CertFilePath, "123456");
            }

            ClientCert = new X509Certificate2(CertFilePath, "123456");
            ClientCertHash = ClientCert.GetCertHashString();

            //初始化管理员
            using var db = new SysDBContext();
            if(db.UserInfo.Count() == 0)
            {
                db.Insert(new UserInfo { 
                    Name = "admin",
                    Password = Way.Lib.AES.Encrypt("admin" , SecretKey),
                    Role = UserInfo_RoleEnum.Admin
                });
            }
        }
    }
}
