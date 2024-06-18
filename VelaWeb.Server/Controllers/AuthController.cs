using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VelaWeb.Server.Controllers
{
    public class AuthController: ControllerBase
    {
        long? _UserId;
        public long UserId
        {
            get
            {
                if (_UserId == null)
                {
                    _UserId = long.Parse(this.User.FindFirstValue("Content").Split(',')[0]);
                }
                return _UserId.GetValueOrDefault();
            }
        }
    }
}
