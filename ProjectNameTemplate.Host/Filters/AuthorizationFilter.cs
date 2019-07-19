using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectNameTemplate.Common.Extensions;
using ProjectNameTemplate.Core;
using ProjectNameTemplate.Host.Attributes;
using Serilog;
using StackExchange.Profiling;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectNameTemplate.Host.Filters
{
    /// <summary>
    /// 登录、权限拦截器
    /// </summary>
    public class AuthorizationFilter : IAsyncAuthorizationFilter
    {
        private ITalkSession session;
        private ILogger Logger;
        public AuthorizationFilter(ITalkSession session,
            ILogger Logger)
        {
            this.Logger = Logger;
            this.session = session;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            session.MiniProfiler = MiniProfiler.StartNew("StartNew");
            session.TrackId = Guid.NewGuid();
            if (context.HttpContext.Request.Headers.Keys.Contains("ParentTrackId"))
            {
                var ParentTrackId = context.HttpContext.Request.Headers["ParentTrackId"].ToString();
                session.ParentTrackId = Guid.Parse(ParentTrackId);
            }
            //开启MiniProfiler
            context.HttpContext.Items.Add("StartNew", session.MiniProfiler);

            //权限验证
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                //控制器上的权限
                var authorizeList = controllerActionDescriptor.
                    ControllerTypeInfo.
                    GetCustomAttributes(true).
                    Where(a => a.GetType().Equals(typeof(AuthorizeAttribute)))
                    .ToList();
                //Action上的权限
                authorizeList.AddRange(controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                  .Where(a => a.GetType().Equals(typeof(AuthorizeAttribute))).ToList());

                var noJsonResultAttribute = controllerActionDescriptor.MethodInfo
                      .GetCustomAttributes(false)
                      .Where(a => a.GetType().Equals(typeof(NoJsonResultAttribute)))
                      .ToList();
                session.NoJsonResult = noJsonResultAttribute.IsAny();

                //只要标注了AuthorizeAttribute，则必须是登录状态
                if (authorizeList.IsAny())
                {
                    // 验证登录
                }

                foreach (var authorize in authorizeList)
                {
                    var permissions = ((AuthorizeAttribute)authorize).Permissions;
                    foreach (var permission in permissions)
                    {
                        //验证权限
                    }
                }
            }
        }
    }
}
