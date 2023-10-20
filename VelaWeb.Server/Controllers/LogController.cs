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
using VelaWeb.Server.Dtos;

namespace VelaWeb.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class LogController : AuthController
    {
        private readonly SysDBContext _db;

        public LogController(SysDBContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<PageResult<LogResponse>> GetLogs(string? searchKey, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            var query = (from m in _db.Logs
                         from u in _db.UserInfo
                         where m.UserId == u.id
                         orderby m.Time descending
                         select new LogResponse
                         {
                             id = m.id,
                             Operation = m.Operation,
                             Detail = m.Detail,
                             Time = m.Time,
                             UserName = u.Name
                         });
            if (startTime != null)
                query = query.Where(m => m.Time >= startTime.Value);

            if (endTime != null)
                query = query.Where(m => m.Time < endTime.Value);

            if (!string.IsNullOrWhiteSpace(searchKey))
                query = query.Where(m => EF.Functions.Like(m.Operation, $"%{searchKey.Trim()}%"));

            var ret = new PageResult<LogResponse>();
            ret.Total = await query.CountAsync();
            ret.Datas = await query.Skip(pageIndex * pageSize).Take(pageSize).ToArrayAsync();
            return ret;
        }
    }
}
