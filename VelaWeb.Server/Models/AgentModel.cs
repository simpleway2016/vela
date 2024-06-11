using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using VelaLib;
using VelaWeb.Server.DBModels;
using VelaWeb.Server.Infrastructures;
using Way.Lib;

namespace VelaWeb.Server.Models
{
    public class AgentModel : IDisposable
    {
        public long id { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string Category { get; set; }
        public string Desc { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        public string LastSendText { get; private set; }

        bool _disposed = false;
        IHttpClientFactory _httpClientFactory;
        WebSocketConnectionCenter _connectionCenter;
        AlarmManager _alarmManager;
        ProjectCenter _projectCenter;
        public override string ToString()
        {
            return $"{Address}:{Port}";
        }

        public void Init()
        {
            _httpClientFactory = Global.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            _connectionCenter = Global.ServiceProvider.GetRequiredService<WebSocketConnectionCenter>();
            _alarmManager = Global.ServiceProvider.GetRequiredService<AlarmManager>();
            _projectCenter = Global.ServiceProvider.GetRequiredService<ProjectCenter>();
            Task.Run(() =>
            {
                SendServerStatusLoop();
            });
        }

        async Task RunCmd(RunningInfo runningInfo, ProjectAlarmSetting setting)
        {
            if (!setting.CanRun)
                return;

            if (setting.LastAlarmTime != null)
            {
                if ((DateTime.Now - setting.LastAlarmTime.Value).TotalMinutes < 10)
                {
                    setting.CanRun = false;
                    return;
                }
            }

            var projectModel = _projectCenter.GetProject(runningInfo.Guid);

            ICmdRunner cmdRunner = Global.ServiceProvider.GetRequiredService<ICmdRunner>();

            setting.CanRun = false;
            setting.LastAlarmTime = DateTime.Now;
            try
            {
                var agentName = this.Category;
                if (string.IsNullOrWhiteSpace(agentName))
                    agentName = this.Desc;

                using var process = cmdRunner.RunInBashWithoutStop(null, setting.Cmd.Replace("%NAME%", projectModel.Name)
                    .Replace("%SERVERNAME%", agentName)
                    .Replace("%CPU%", runningInfo.CpuPercent + "%").Replace("%MEMORY%", runningInfo.MemoryPercent + "%"));
            }
            catch (Exception ex)
            {

            }
        }

        async void checkRunningInfo(string json)
        {
            try
            {
                var settings = _alarmManager.GetAllSettings();
                if (settings.Length == 0)
                    return;

                var infos = json.FromJson<RunningInfo[]>();

                foreach (var info in infos)
                {
                    foreach (var setting in settings)
                    {
                        if (setting.ProjectGuid != "global" && setting.ProjectGuid != info.Guid || setting.IsEnable == false)
                            continue;

                        var project = _projectCenter.GetProject(info.Guid);
                        var starttime = _projectCenter.GetProjectStartTime(info.Guid);

                        if (project == null || (DateTime.Now - starttime).TotalMinutes < 2)
                        {
                            continue;
                        }

                        if (setting.Cpu > 0 && info.CpuPercent >= setting.Cpu)
                        {
                            await RunCmd(info, setting);
                            return;
                        }
                        else if (setting.Memory > 0 && info.MemoryPercent >= setting.Memory)
                        {
                            await RunCmd(info, setting);
                            return;
                        }
                        else
                        {
                            if (setting.CanRun == false)
                            {
                                //让它下一次超过报警线可以执行命令
                                setting.CanRun = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Global.ServiceProvider.GetRequiredService<ILogger<AgentModel>>()?.LogError(ex, "");
            }
        }

        /// <summary>
        /// 不断获取程序的cpu、内存等状态
        /// </summary>
        async void SendServerStatusLoop()
        {
            if (_disposed)
                return;

            var client = _httpClientFactory.CreateClient("");
            Agent agentItem;
            using (var db = new SysDBContext())
            {
                agentItem = db.Agent.FirstOrDefault(m => m.id == this.id);
               
            }


            try
            {
                var ret = await client.GetStringAsync($"https://{agentItem.Address}:{agentItem.Port}/Publish/GetAllProjectRunningInfos");
                checkRunningInfo(ret);
                LastSendText = ret;
                await _connectionCenter.SendTextAsync(ret);
            }
            catch (Exception ex)
            {
                await _connectionCenter.SendTextAsync(new { IsError = true, Server = agentItem }.ToJsonString());
            }
            finally
            {
                await Task.Delay(3000);
                SendServerStatusLoop();
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
