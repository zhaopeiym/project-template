using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProjectNameTemplate.Host.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        ILogger Logger;
        public ExceptionFilter()
        {
            Logger = Log.Logger;
        }

        public void OnException(ExceptionContext context)
        {
            var requestUrl = context.HttpContext.Request.Path.Value;
            Logger.Error(context.Exception, $"OnException - HashCode:{GetHashCode()} Url:{requestUrl} Err:{context.Exception.Message}");

            context.Result = new JsonResult(new
            {
                IsSuccess = false,
                State = 0,
                ErrorMsg = context.Exception.Message
            });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.ExceptionHandled = true;
        }
    }
}
