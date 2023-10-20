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

            var dirs = Directory.GetDirectories(backupFolder).OrderBy(m=>m).Skip(5).ToArray();

            foreach ( var subDir in dirs)
            {
                try
                {
                    var folderName = Path.GetFileName(subDir);
                    var time = DateTimeOffset.FromUnixTimeMilliseconds( long.Parse(folderName)).UtcDateTime;
                    if( (DateTime.UtcNow - time).TotalDays > 5)
                    {
                        SysUtility.DeleteFolder(subDir);
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
