using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProjectNameTemplate.Host.Filters
{
    public class ActionFilter : IAsyncActionFilter
    {
        ILogger Logger;

        public ActionFilter()
        {

            Logger = Log.Logger;
        }

        /// <summary>
        /// ActionExecution
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();//开始监视代码运行时间

            #region 执行前

            var inputList = context.ActionDescriptor.Parameters.Select(t => context.ActionArguments.Keys.Contains(t.Name) ? JsonConvert.SerializeObject(context.ActionArguments[t.Name]) : string.Empty).ToList();
            var parameterStr = string.Join(" ", inputList);
            var requestUrl = context.HttpContext.Request.Path.Value;
            Logger.Debug($"ActionBegin - HashCode:{GetHashCode()} Url:{requestUrl} Parameter:{parameterStr}");

            if (!context.ModelState.IsValid)
            {
                var errList = context.ModelState.Values.SelectMany(t => t.Errors.Select(e => string.IsNullOrWhiteSpace(e.Exception?.Message) ? e.ErrorMessage : e.Exception?.Message)).ToList();

                Logger.Error($"实体验证失败 - HashCode:{GetHashCode()} {string.Join(" ", errList)}");

                context.Result = new JsonResult(new
                {
                    ErrorList = errList.ToList(),
                    IsSuccess = false,
                    State = 400,
                    ErrorMsg = string.Join(" ", errList).Trim()
                });
                return;
            }
            #endregion

            //执行
            var actionExecutedContext = await next();

            #region 执行之后
            stopwatch.Stop(); //  停止监视 
            double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
            if (actionExecutedContext.Exception == null)
            {
                dynamic contextResult = actionExecutedContext.Result ?? new EmptyResult();
                var result = new
                {
                    IsSuccess = true,
                    State = 1,
                    Data = actionExecutedContext?.Result
                };

                //actionExecutedContext.Result = contextResult.Value is ResponseBase ? new JsonResult(contextResult.Value) : new JsonResult(result);
                var resultStr = JsonConvert.SerializeObject(result);
                var maxLenght = resultStr.Length > 1000 ? 1000 : resultStr.Length;
                Logger.Debug($"ActionEnd - HashCode:{GetHashCode()} 耗时:{seconds}秒 Result:{resultStr.Substring(0, maxLenght)}");

                if (seconds > 1)//大于一秒的请求记录警告日志 
                    Logger.Warning($"ActionEnd - HashCode:{GetHashCode()} 耗时:{seconds}秒 Result:{resultStr.Substring(0, maxLenght)}");
            }
            else
            {
                // 自定义的逻辑处理友好异常              
                //if (actionExecutedContext.Exception is UserFriendlyException)  

                #region 未知异常
                //未知异常             
                var ErrMsg = "服务器异常，请稍后再试。";
#if DEBUG
                //（只有在开发环境才抛出详细异常信息）
                ErrMsg = actionExecutedContext.Exception.Message;
#endif
                Logger.Error(actionExecutedContext.Exception, $@"OnException - HashCode:{GetHashCode()}   
                                                                    耗时:{seconds}秒 
                                                                    Url:{requestUrl}                                                                  
                                                                    Err:{actionExecutedContext.Exception.Message}");

                actionExecutedContext.Result = new JsonResult(new
                {
                    IsSuccess = false,
                    State = 500,
                    ErrorMsg = ErrMsg
                });
                #endregion

                //actionExecutedContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //除了200,其他状态码契约调用都会返回null，也就是异常显示不了。
                actionExecutedContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                actionExecutedContext.ExceptionHandled = true;
            }

            #endregion
        }
    }
}
