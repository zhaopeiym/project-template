using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using ProjectNameTemplate.Common.Extensions;
using ProjectNameTemplate.Core;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Talk;
using Talk.Contract;

namespace ProjectNameTemplate.WebApi.Filters
{
    public class ActionFilter : IAsyncActionFilter
    {
        private ITalkLogger Logger;
        private ITalkSession session;
        public ActionFilter(ITalkSession session,
            ITalkLogger Logger)
        {
            this.Logger = Logger;
            this.session = session;
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
            var isWebApi = false;
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                //控制器上的权限
                var authorizeList = controllerActionDescriptor.
                    ControllerTypeInfo.
                    GetCustomAttributes(true).
                    Where(a => a.GetType().Equals(typeof(ApiControllerAttribute)))
                    .ToList();
                isWebApi = authorizeList.IsAny();
            }

            #region 执行前
            if (context.ActionDescriptor.RouteValues.Keys.Contains("action"))
                session.ActionName = context.ActionDescriptor.RouteValues["action"];
            if (context.ActionDescriptor.RouteValues.Keys.Contains("controller"))
                session.ControllerName = context.ActionDescriptor.RouteValues["controller"];
            var inputList = context.ActionDescriptor.Parameters.Select(t => context.ActionArguments.Keys.Contains(t.Name) ? JsonConvert.SerializeObject(context.ActionArguments[t.Name]) : string.Empty).ToList();
            var parameterStr = string.Join(" ", inputList);
            var requestUrl = context.HttpContext.Request.Path.Value;
            session.RequestUrl = requestUrl;
            Logger.Debug($"ActionBegin - HashCode:{GetHashCode()} Url:{requestUrl} Parameter:{parameterStr}");

            if (!context.ModelState.IsValid)
            {
                var errList = context.ModelState.Values.SelectMany(t => t.Errors.Select(e => string.IsNullOrWhiteSpace(e.Exception?.Message) ? e.ErrorMessage : e.Exception?.Message)).ToList();

                Logger.Error($"实体验证失败 - HashCode:{GetHashCode()} {string.Join(" ", errList)}");
                if (isWebApi)
                    context.Result = new JsonResult(new ResultBase()
                    {
                        ErrorList = errList.ToList(),
                        Code = HttpCodeEnum.C412,
                        ErrorMsg = string.Join(" ", errList).Trim(),
                        TrackId = session.TrackId
                    });
                return;
            }
            #endregion

            #region 执行，并监控执行sql 
            var profiler = context.HttpContext.Items["StartNew"] as MiniProfiler;
            ActionExecutedContext actionExecutedContext = null;
            using (profiler.Step("Level1"))
            {
                actionExecutedContext = await next();
            }
            WriteLog(profiler);
            if (profiler != null) await profiler?.StopAsync(true);
            #endregion

            #region 执行之后
            stopwatch.Stop(); //  停止监视 
            double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
            if (actionExecutedContext.Exception == null)
            {
                dynamic contextResult = actionExecutedContext.Result ?? new EmptyResult();
                if (contextResult is FileStreamResult)//文件流
                    return;
                var result = new ResultBase<dynamic>()
                {                    
                    Data = (contextResult is EmptyResult) ? null : isWebApi ? contextResult.Value : null, //actionExecutedContext?.Result,
                    TrackId = session.TrackId
                };
                //if (isWebApi)
                //    actionExecutedContext.Result = new JsonResult(result);
                if (!session.NoJsonResult)
                    actionExecutedContext.Result = new JsonResult(result);
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
                if (isWebApi)
                    actionExecutedContext.Result = new JsonResult(new ResultBase()
                    {
                        Code = HttpCodeEnum.C500,
                        ErrorMsg = ErrMsg,
                        TrackId = session.TrackId
                    });
                #endregion

                //actionExecutedContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //除了200,其他状态码契约调用都会返回null，也就是异常显示不了。
                actionExecutedContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                actionExecutedContext.ExceptionHandled = true;
            }

            #endregion
        }

        /// <summary>
        /// sql跟踪
        /// </summary>
        /// <param name="profiler"></param>
        private void WriteLog(MiniProfiler profiler)
        {
            if (profiler?.Root != null)
            {
                var root = profiler.Root;
                if (root.HasChildren)
                {
                    root.Children.ForEach(chil =>
                    {
                        if (chil.CustomTimings?.Count > 0)
                        {
                            foreach (var customTiming in chil.CustomTimings)
                            {
                                var all_sql = new List<string>();
                                var err_sql = new List<string>();
                                var all_log = new List<string>();
                                int i = 1;
                                customTiming.Value?.ForEach(value =>
                                {
                                    if (value.ExecuteType != "OpenAsync")
                                        all_sql.Add(value.CommandString.Replace("\r", "").Replace("\n", "").Replace("\t", "")
                                            .Replace("      ", " ").Replace("     ", " ")
                                            .Replace("    ", " ").Replace("   ", " ")
                                            .Replace("  ", " "));
                                    if (value.Errored)
                                        err_sql.Add(value.CommandString.Replace("\r", "").Replace("\n", "").Replace("\t", "")
                                            .Replace("      ", " ").Replace("     ", " ")
                                            .Replace("    ", " ").Replace("   ", " ")
                                            .Replace("  ", " "));
                                    var log = $@"【{customTiming.Key}{i++}】{value.CommandString.Replace("\r", "").Replace("\n", "").Replace("\t", "")
                                            .Replace("      ", " ").Replace("     ", " ")
                                            .Replace("    ", " ").Replace("   ", " ")
                                            .Replace("  ", " ")} Execute time :{value.DurationMilliseconds} ms,Start offset :{value.StartMilliseconds} ms,Errored :{value.Errored}";
                                    all_log.Add(log);
                                });

                                if (err_sql.IsAny())
                                    Logger.Error(new Exception("sql异常"), "异常sql:\r\n" + err_sql.StringJoin("\r\n"), sql: err_sql.StringJoin("\r\n\r\n"));
                                Logger.Debug(all_log.StringJoin("\r\n"), sql: all_sql.StringJoin("\r\n\r\n"));
                            }
                        }
                    });
                }
            }
        }
    }
}
