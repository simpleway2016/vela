using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Security.Claims;
using VelaWeb.Server.DBModels;
using Way.Lib;

namespace VelaWeb.Server
{
    public class PermissionFilter : IActionFilter
    {
        private readonly IMemoryCache _memoryCache;

        public PermissionFilter(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
           
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User == null)
                return;

            var arr = context.HttpContext.User.FindFirstValue("Content")?.Split(',');
            if (arr == null)
                return;

            var userid = long.Parse(arr[0]);
            var flag = int.Parse(arr[1]);


            if( _memoryCache.TryGetValue($"{userid}_flag" , out int o))
            {
                if(flag != o)
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = 401,
                        Content = "The user has already logged in on another device"
                    };
                    return;
                }
            }
        }
    }
}
