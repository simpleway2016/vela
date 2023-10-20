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

namespace VelaWeb.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : AuthController
    {
        private readonly SysDBContext _db;
        private readonly TokenClient _tokenClient;
        private readonly ProjectCenter _projectCenter;
        private readonly AgentsManager _agentsManager;

        public UserController(SysDBContext db, TokenClient tokenClient, ProjectCenter projectCenter,AgentsManager agentsManager)
        {
            _db = db;
            _tokenClient = tokenClient;
            _projectCenter = projectCenter;
            _agentsManager = agentsManager;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<string> Login([FromBody] LoginRequestModel request)
        {
            var pwd = Way.Lib.AES.Encrypt(request.Password, Global.SecretKey);
            var userinfo = await _db.UserInfo.FirstOrDefaultAsync(m => m.Name == request.Name);
            if (userinfo == null)
                throw new ServiceException("用户名、密码错误");

            if (userinfo.IsLock)
                throw new ServiceException("账号已被锁定");

            if (userinfo.Password != pwd)
            {
                userinfo.ErrorCount++;
                if(userinfo.ErrorCount >= 10)
                {
                    userinfo.IsLock = true;
                }
                await _db.UpdateAsync(userinfo);
                throw new ServiceException("用户名、密码错误");
            }

            userinfo.ErrorCount = 0;
            await _db.UpdateAsync(userinfo);

#if DEBUG
            return _tokenClient.Build(userinfo.id.ToString(), userinfo.Role.ToString(), DateTime.Now.AddMinutes(10000));
#else
            return _tokenClient.Build(userinfo.id.ToString() ,userinfo.Role.ToString(), DateTime.Now.AddMinutes(10));
#endif
        }

        [HttpGet]
        public string RefreshToken()
        {
            var obj = _tokenClient.Verify(Request.Headers.Authorization.ToString());
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = this.User.FindFirstValue(ClaimTypes.Role);
#if DEBUG
            return _tokenClient.Build(userid, role, DateTime.Now.AddMinutes(10000));
#else
            return _tokenClient.Build(userid, role,  DateTime.Now.AddMinutes(10));
#endif
        }

        [HttpGet]
        public async Task<DBModels.UserInfo> GetUserInfo()
        {
            var userid = long.Parse(this.User.FindFirstValue("Content"));
            var ret = await _db.UserInfo.FirstOrDefaultAsync(m => m.id == userid);
            ret.Password = typeof(UserController).Assembly.GetName().Version.ToString();
            ret.ChangedProperties.Clear();
            return ret;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task ChangePassword([FromForm] string password, [FromForm]string oldPwd)
        {
            oldPwd = Way.Lib.AES.Encrypt(oldPwd, Global.SecretKey);
            var user = await _db.UserInfo.FirstOrDefaultAsync(m => m.id == this.UserId);
            if (user.Password != oldPwd)
                throw new ServiceException("旧密码错误");

            user.Password = Way.Lib.AES.Encrypt(password, Global.SecretKey);
            await _db.UpdateAsync(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<UserInfo[]> GetSystemUsers()
        {
            var users = await _db.UserInfo.ToArrayAsync();
            foreach( var user in users)
            {
                user.Password = null;
                user.ChangedProperties.Clear();
            }
            
            return users;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<long> AddUser([FromBody]UserInfo requestModel)
        {
            requestModel.Password = Way.Lib.AES.Encrypt(requestModel.Password, Global.SecretKey);
            requestModel.Name = requestModel.Name.Trim();

            if (string.IsNullOrWhiteSpace(requestModel.Name))
                throw new ServiceException("缺少用户名");

            if (await _db.UserInfo.AnyAsync(m => m.Name == requestModel.Name))
                throw new ServiceException("用户名重复");

            await _db.InsertAsync(requestModel);

            await _db.InsertAsync(new Logs
            {
                UserId = this.UserId,
                Operation = "添加用户",
                Time = DateTime.UtcNow,
                Detail = requestModel.Name
            });

            return requestModel.id.Value;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task ModifyUser( [FromBody] UserInfo requestModel)
        {
            if (string.IsNullOrWhiteSpace(requestModel.Name))
                throw new ServiceException("缺少用户名");

            var user = await _db.UserInfo.FirstOrDefaultAsync(m=>m.id == requestModel.id);
            user.Name = requestModel.Name;
            if(!string.IsNullOrEmpty(requestModel.Password))
            {
                user.Password = Way.Lib.AES.Encrypt( requestModel.Password , Global.SecretKey);
            }
            user.Role = requestModel.Role;
            user.IsLock = false;

            await _db.InsertAsync(new Logs
            {
                UserId = this.UserId,
                Operation = "修改用户",
                Time = DateTime.UtcNow,
                Detail = user.Name
            });


            await _db.UpdateAsync(user);

           
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task DeleteUser(long id)
        {
            _db.BeginTransaction();
            var user = await _db.UserInfo.FirstOrDefaultAsync(m=>m.id == id);
            await _db.DeleteAsync<UserInfo>(m=>m.id == id);
            await _db.DeleteAsync<UserProjectPower>(m => m.UserId == id);

            await _db.InsertAsync(new Logs
            {
                UserId = this.UserId,
                Operation = "删除用户",
                Time = DateTime.UtcNow,
                Detail = user.Name + " : " + user.id
            }) ;

            _db.CommitTransaction();
        }

        /// <summary>
        /// 授权什么用户可以操作指定工程
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="userids"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        [HttpPost]
        public async Task SetProjectPowers([FromForm]string guid, [FromForm] long[] userids)
        {
            var role = this.User.FindFirstValue(ClaimTypes.Role);
            var project = _projectCenter.GetProject(guid);
            if (role != "Admin" && project.UserId != this.UserId)
                throw new ServiceException($"您无权授权{project.Name}");

            _db.BeginTransaction();

            await _db.DeleteAsync<UserProjectPower>(m => m.ProjectGuid == guid);

            foreach( var userid in userids)
            {
                await _db.InsertAsync(new UserProjectPower
                {
                    UserId = userid,
                    ProjectGuid = guid
                });

                await _db.InsertAsync(new Logs
                {
                    UserId = this.UserId,
                    Operation = "给用户授权",
                    Time = DateTime.UtcNow,
                    Detail = $"{project.Name}  UserId:{userid}"
                });
            }
            _db.CommitTransaction();
        }

        /// <summary>
        /// 获取哪些用户有这个工程的权限
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<UserInfo[]> GetProjectOwnerUsers(string guid)
        {
            var users = await _db.UserInfo.Where(m=>m.Role != UserInfo_RoleEnum.Admin).OrderBy(m=>m.Name).ToArrayAsync();
            foreach( var user in users)
            {
                user.Password = null;
                if(await _db.UserProjectPower.AnyAsync(m=>m.UserId == user.id && m.ProjectGuid == guid))
                {
                    user.Password = "1";//表示用户有权限
                }
                user.ChangedProperties.Clear();
            }
            return users;
        }

        /// <summary>
        /// 授权什么用户可以在服务器部署程序
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="userids"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task SetAgentPowers([FromForm] long agentId, [FromForm] long[] userids)
        {
          

            var agents = _agentsManager.GetAllAgents();
            var agent = agents.FirstOrDefault(m=>m.id == agentId);

            _db.BeginTransaction();

            await _db.DeleteAsync<UserAgentPower>(m => m.AgentId == agentId);

            foreach (var userid in userids)
            {
                await _db.InsertAsync(new UserAgentPower
                {
                    UserId = userid,
                    AgentId = agentId
                });

                await _db.InsertAsync(new Logs
                {
                    UserId = this.UserId,
                    Operation = "给用户授权部署服务器",
                    Time = DateTime.UtcNow,
                    Detail = $"{agent.ToString()}  UserId:{userid}"
                });
            }
            _db.CommitTransaction();
        }

        /// <summary>
        /// 获取哪些用户有这个agent的权限
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<UserInfo[]> GetAgentOwnerUsers(long agentId)
        {
            var users = await _db.UserInfo.Where(m => m.Role != UserInfo_RoleEnum.Admin).OrderBy(m => m.Name).ToArrayAsync();
            foreach (var user in users)
            {
                user.Password = null;
                if (await _db.UserAgentPower.AnyAsync(m => m.UserId == user.id && m.AgentId == agentId))
                {
                    user.Password = "1";//表示用户有权限
                }
                user.ChangedProperties.Clear();
            }
            return users;
        }
    }
}
