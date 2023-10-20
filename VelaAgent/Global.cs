using JMS;
using Microsoft.Extensions.Configuration.Json;
using System.Text;
using VelaAgent.Dtos;
using Way.Lib;

namespace VelaAgent
{
    public class Global
    {
        public static IConfiguration Configuration { get; set; }
        public static IServiceProvider ServiceProvider { get; set; }

        static JMS.Common.ConfigurationValue<AppConfig> _AppConfig;
        public static JMS.Common.ConfigurationValue<AppConfig> AppConfig
        {
            get {
                return _AppConfig ??= Configuration.GetNewest<AppConfig>();
            }
        }


        static string _ClientCertHash;
        public static string ClientCertHash
        {
            get
            {
                if(_ClientCertHash == null)
                {
                    var filepath = "./data.ClientCertHash";
                    if (File.Exists(filepath))
                    {
                        _ClientCertHash = File.ReadAllText(filepath, Encoding.UTF8);
                    }
                    else
                    {
                        _ClientCertHash = "";
                    }
                }
                return _ClientCertHash;
            }
            set
            {
                if(_ClientCertHash != value)
                {
                    var filepath = "./data.ClientCertHash";
                    File.WriteAllText(filepath, value, Encoding.UTF8);
                    _ClientCertHash = value;
                }
            }
        }

    }

    public class AppConfig
    {
        public string PublishRootPath { get; set; }
        public string BackupPath { get; set; } = "./ProjectBackups";
        public string FileListFolder { get; set; } = "FileList";
    }
}
