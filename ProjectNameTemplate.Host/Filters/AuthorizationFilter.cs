﻿using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectNameTemplate.Common.Extensions;
using ProjectNameTemplate.Core;
using Serilog;
using StackExchange.Profiling;
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
            //开启MiniProfiler
            context.HttpContext.Items.Add("StartNew", MiniProfiler.StartNew("StartNew"));

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