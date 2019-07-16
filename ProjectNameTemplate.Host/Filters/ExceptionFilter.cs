using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectNameTemplate.Core;
using System.Net;
using Talk;
using Talk.Contract;

namespace ProjectNameTemplate.Host.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private ITalkLogger Logger;
        public ExceptionFilter(ITalkLogger Logger)
        {
            this.Logger = Logger;
        }

        public void OnException(ExceptionContext context)
        {
            var requestUrl = context.HttpContext.Request.Path.Value;
            Logger.Error(context.Exception, $"OnException - HashCode:{GetHashCode()} Url:{requestUrl} Err:{context.Exception.Message} { context.Exception.StackTrace}");

            context.Result = new JsonResult(new ResultBase()
            {
                Code = HttpCodeEnum.C500,
                ErrorMsg = context.Exception.Message + " " + context.Exception.StackTrace
            });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.ExceptionHandled = true;
        }
    }
}
