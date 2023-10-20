using System.Collections.Concurrent;
using VelaWeb.Server.DBModels;

namespace VelaWeb.Server
{
    public class AlarmManager
    {
        private readonly ProjectCenter _projectCenter;
        ConcurrentDictionary<long, ProjectAlarmSetting> _allSettings = new ConcurrentDictionary<long, ProjectAlarmSetting>();
        public AlarmManager(ProjectCenter projectCenter)
        {
            _projectCenter = projectCenter;
        }

        public ProjectAlarmSetting[] GetAllSettings()
        {
            return (from m in _allSettings
                    select m.Value).ToArray();
        }

        public void Init()
        {
            using var db = new SysDBContext();
            var settings = db.ProjectAlarmSetting.ToArray();
            foreach (var setting in settings)
            {
                _allSettings[setting.id.Value] = setting;
            }
        }

        public void OnNewSetting(ProjectAlarmSetting setting)
        {
            setting.CanRun = true;
            setting.LastAlarmTime = null;
            _allSettings[setting.id.Value] = setting;
        }
        public void OnSettingChanged(ProjectAlarmSetting setting)
        {
            setting.CanRun = true;
            setting.LastAlarmTime = null;
            _allSettings[setting.id.Value] = setting;
        }
        public void OnSettingDeleted(long id)
        {
            _allSettings.TryRemove(id, out _);
        }
    }
}
