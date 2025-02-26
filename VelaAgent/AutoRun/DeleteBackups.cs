using System.Linq;
using VelaAgent.DBModels;
using VelaLib;

namespace VelaAgent.AutoRun
{
    public class DeleteBackups
    {
        private readonly ILogger<DeleteBackups> _logger;

        public DeleteBackups(ILogger<DeleteBackups> logger)
        {
            _logger = logger;
        }
        public void Run()
        {
            new Thread(runOnBackground).Start();
        }

        void runOnBackground()
        {
            while (true)
            {
                try
                {
                    using var db = new SysDBContext();
                    var projects = db.Project.ToArray();
                    foreach( var project in projects)
                    {
                        deleteProjectBackups(project);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "");
                }
                Thread.Sleep(60000);
            }
        }

        void deleteProjectBackups(Project project)
        {
            var backupFolder = Path.Combine(Global.AppConfig.Current.BackupPath, project.Guid);
            if (Directory.Exists(backupFolder) == false)
                return;
            if (project.BackupCount >= 0)
            {
                var folderObjects = Directory.GetDirectories(backupFolder).Select(m => new BackupFolder(m)).OrderByDescending(m => m.Time).Skip(project.BackupCount.Value).ToArray();
                foreach (var folderObj in folderObjects)
                {
                    try
                    {
                        SysUtility.DeleteFolder(folderObj.Dir);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "");
                    }
                }
            }
            else
            {
                var folderObjects = Directory.GetDirectories(backupFolder).Select(m => new BackupFolder(m)).OrderByDescending(m => m.Time).Skip(5).ToArray();

                foreach (var folderObj in folderObjects)
                {
                    try
                    {
                        if ((DateTime.UtcNow - folderObj.Time).TotalDays > Global.AppConfig.Current.BackupKeepDays)
                        {
                            SysUtility.DeleteFolder(folderObj.Dir);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "");
                    }
                }
            }
        }
    }

    class BackupFolder
    {
        public string Dir;
        public DateTime Time;
        public BackupFolder(string dir)
        {
            Dir = dir;
            var folderName = Path.GetFileName(dir);
            Time = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(folderName)).UtcDateTime;
        }
    }
}
