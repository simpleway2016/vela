using System.Threading.Tasks;
using VelaLib;
using VelaWeb.Server.DBModels;

namespace VelaWeb.Server.AutoRun
{
    public class DeleteLogs
    {
        private readonly ILogger<DeleteLogs> _logger;

        public DeleteLogs(ILogger<DeleteLogs> logger)
        {
            _logger = logger;
        }
        public async Task StartAsync()
        {
            while (true)
            {
                await Task.Delay(600000);
                try
                {
                    var time = DateTime.Now.AddDays(-30);
                    using var db = new SysDBContext();
                    db.Delete<Logs>(m => m.Time < time);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "");
                }
            }
        }
    }
}
