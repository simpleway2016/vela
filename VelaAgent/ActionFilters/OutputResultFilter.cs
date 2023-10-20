using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VelaAgent.ActionFilters
{
    public class OutputResultFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.Exception != null && context.HttpContext.WebSockets.IsWebSocketRequest == false)
            {
                var err = context.Exception;
                context.Exception = null;
                context.ExceptionHandled = true;
                while (err.InnerException != null)
                    err = err.InnerException;

                if (err is ServiceException)
                {

                }
                else
                {
                    context.HttpContext.RequestServices.GetService<ILogger<OutputResultFilter>>().LogError(err, "");
                }

                context.HttpContext.Response.StatusCode = 500;
                context.Result = new  ObjectResult(err.Message); 
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }
    }
}
