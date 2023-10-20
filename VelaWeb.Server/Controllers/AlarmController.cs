using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using VelaWeb.Dtos;
using VelaWeb.Server.DBModels;
using VelaWeb.Dtos;
using Way.Lib;
using JMS.Token;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Xml.Linq;
using EJ;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VelaWeb.Server.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class AlarmController : AuthController
    {
        private readonly SysDBContext _db;
        private readonly ProjectCenter _projectCenter;
        private readonly AlarmManager _alarmManager;

        public AlarmController(SysDBContext db, ProjectCenter projectCenter,AlarmManager alarmManager)
        {
            _db = db;
            _projectCenter = projectCenter;
            _alarmManager = alarmManager;
        }

        [HttpPost]
        public async Task<long> AddAlarmSetting([FromBody] ProjectAlarmSetting setting)
        {
            if (string.IsNullOrWhiteSpace(setting.Cmd))
                throw new ServiceException("请填写执行命令");

            if (setting.Cpu != null && setting.Cpu < 0)
                throw new ServiceException("请填写正确的cpu阈值");

            if (setting.Memory != null && setting.Memory < 0)
                throw new ServiceException("请填写正确的内存阈值");
                     

            await _db.InsertAsync(setting);
            _alarmManager.OnNewSetting(setting);

            return setting.id.Value;
        }

        [HttpPost]
        public async Task ModifyAlarmSetting([FromBody] ProjectAlarmSetting setting)
        {
            if (string.IsNullOrWhiteSpace(setting.Cmd))
                throw new ServiceException("请填写执行命令");

            if (setting.Cpu <= 0)
                throw new ServiceException("请填写正确的cpu阈值");

            if (setting.Memory <= 0)
                throw new ServiceException("请填写正确的内存阈值");

            var data = await _db.ProjectAlarmSetting.FirstOrDefaultAsync(m => m.id == setting.id);
            setting.CopyValueTo(data, true, false);
            await _db.UpdateAsync(data);

            _alarmManager.OnSettingChanged(data);
        }

        [HttpGet]
        public async Task DeleteAlarmSetting(long id)
        {
            await _db.DeleteAsync<ProjectAlarmSetting>(x=>x.id == id);

            _alarmManager.OnSettingDeleted(id);
        }

        [HttpGet]
        public async Task<ProjectAlarmSetting[]> GetAlarmSettings(string guid)
        {
            return await _db.ProjectAlarmSetting.Where(m=>m.ProjectGuid == guid).ToArrayAsync();
        }
    }
}
