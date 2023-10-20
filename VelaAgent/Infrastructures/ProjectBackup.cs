using Microsoft.EntityFrameworkCore;
using System.Text;
using VelaAgent.DBModels;
using VelaLib;
using Way.Lib;

namespace VelaAgent.Infrastructures
{
    /// <summary>
    /// 程序备份
    /// </summary>
    public class ProjectBackup
    {
        /// <summary>
        /// 创建一个备份
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public async Task Create(SysDBContext db, Project project, string velaFileListPath, string[] fileList)
        {
            var publishPath = Path.Combine(Global.AppConfig.Current.PublishRootPath, project.Name);
            if (!string.IsNullOrWhiteSpace(project.PublishPath))
            {
                publishPath = project.PublishPath;
            }

            var backupFolder = Path.Combine(Global.AppConfig.Current.BackupPath, project.Guid, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            if (Directory.Exists(backupFolder) == false)
            {
                Directory.CreateDirectory(backupFolder);
            }

            foreach (var filePath in fileList)
            {
                var sourcePath = Path.Combine(publishPath, filePath);
                var targetPath = Path.Combine(backupFolder, filePath);
                var targetdir = Path.GetDirectoryName(targetPath);
                if (Directory.Exists(targetdir) == false)
                    Directory.CreateDirectory(targetdir);
                File.Copy(sourcePath, targetPath, true);
            }
            File.Copy(velaFileListPath, Path.Combine(backupFolder, $"vela.filelist.{project.Guid}.json"), true);
        }

        public async Task Restore(SysDBContext db, string guid, string targetFolder, string backupFilePath, IInfoOutput output)
        {

            var backupFolder = Path.Combine(Global.AppConfig.Current.BackupPath, guid, backupFilePath);
            var velaFileListPath = Path.Combine(backupFolder, $"vela.filelist.{guid}.json");

            if (File.Exists(velaFileListPath) == false)
            {
                throw new Exception("这是老版本生成的备份，已不能使用");
            }
            string[] fileList = File.ReadAllText(velaFileListPath, Encoding.UTF8).FromJson<string[]>();
            foreach (var filePath in fileList)
            {
                var sourcePath = Path.Combine(backupFolder, filePath);
                var targetPath = Path.Combine(targetFolder, filePath);

                var targetdir = Path.GetDirectoryName(targetPath);
                if (Directory.Exists(targetdir) == false)
                    Directory.CreateDirectory(targetdir);

                if (File.Exists(targetPath))
                {
                    SysUtility.DeleteFile(targetPath);
                }
                File.Copy(sourcePath, targetPath, true);
            }
            File.Copy(velaFileListPath, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Global.AppConfig.Current.FileListFolder, guid), true);

        }
    }
}
