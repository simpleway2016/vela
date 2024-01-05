using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using VelaLib;
using Way.Lib;

namespace VelaWeb.Server.Infrastructures
{
    public interface IUpgradePackageService
    {
        /// <summary>
        /// 检查压缩包是否是合格的升级包
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        bool CheckPackage(string filepath);
        string GetVersion(bool readCache = true);
        bool IsWebServerPackage(string filepath);
        void UpgradeWebServer(string filepath);
    }

    class DefaultUpgradePackageService : IUpgradePackageService
    {
        public DefaultUpgradePackageService(IFileService fileService)
        {
            _fileService = fileService;
        }
        static string CurrentVersion;
        private readonly IFileService _fileService;

        public bool CheckPackage(string filepath)
        {
            if(OperatingSystem.IsWindows() == false)
            {
                if (filepath.Contains(".win."))
                {
                    return false;
                }
            }

            using (ZipArchive archive = ZipFile.OpenRead(filepath))
            {
                // 遍历 ZIP 文件的每个条目
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName == "VelaAgent.dll")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void UpgradeWebServer(string filepath)
        {
            System.IO.File.Move(filepath, "./Upgrade.zip", true);

            var model = new VelaService.ServiceUpgradeConfigModel()
            {
                Zip = "Upgrade.zip",
                ExcludeFiles = new string[] { "appsettings.json", "VelaWeb.Server", "createdump", "VelaService" }
            };
            System.IO.File.WriteAllText("./VelaService.upgrade.json", model.ToJsonString(), Encoding.UTF8);

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                Process.GetCurrentProcess().Kill();
            });
        }

        public bool IsWebServerPackage(string filepath)
        {
            using (ZipArchive archive = ZipFile.OpenRead(filepath))
            {
                // 遍历 ZIP 文件的每个条目
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName == "VelaWeb.Server.dll")
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public string GetVersion(bool readCache = true)
        {
            if (File.Exists(Global.AgentUpgradeFilePath) == false)
                return null;

            if (readCache && CurrentVersion != null)
                return CurrentVersion;

            using (ZipArchive archive = ZipFile.OpenRead(Global.AgentUpgradeFilePath))
            {
                // 遍历 ZIP 文件的每个条目
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName == "VelaAgent.dll")
                    {
                        var filepath = $"./{Guid.NewGuid().ToString("N")}";
                        entry.ExtractToFile(filepath);
                        CurrentVersion = FileVersionInfo.GetVersionInfo(filepath).FileVersion;
                        SysUtility.DeleteFile(filepath);
                        return CurrentVersion;
                    }
                }
            }
            throw new ServiceException("无效的更新包");
        }
    }
}
