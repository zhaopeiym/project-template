﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutofacSerilogIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using ProjectNameTemplate.Host.Filters;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Talk;
using Talk.AutoMap.Extensions;

namespace ProjectNameTemplate.Host
{
    /// <summary>
    /// 模版来源：
    /// https://github.com/zhaopeiym/Boilerplate/issues
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 日志配置
            LogConfig();

            #region 跨域
#if DEBUG
            services.AddCors(options =>
                options.AddPolicy("AllowSameDomain",
                     builder => builder
                     .WithOrigins("http://example.com", "http://www.contoso.com")
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     //.AllowAnyOrigin()    //允许任何来源的主机访问（debug开发允许跨域）
                     .AllowCredentials()  //指定处理cookie
                     ));
#else
            services.AddCors(options => options.AddPolicy("AllowSameDomain", builder => { }));        
#endif
            #endregion

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //替换控制器所有者  http://www.cnblogs.com/GuZhenYin/p/8301500.html
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            //https://docs.microsoft.com/zh-cn/aspnet/core/web-api/?view=aspnetcore-2.1
            services.Configure<ApiBehaviorOptions>(options =>
            {
                //options.SuppressConsumesConstraintForFormFileParameters = true;   //关闭请求multipart推断
                //options.SuppressInferBindingSourcesForParameters = true;          //关闭类型参数推断
                options.SuppressModelStateInvalidFilter = true;   //关闭自动验证对象属性并处理
            });

            //services.AddMvc(options =>
            //{
            //    options.Filters.Add<AuthorizationFilter>();
            //    options.Filters.Add<ActionFilter>();
            //    options.Filters.Add<ExceptionFilter>();
            //}).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<AuthorizationFilter>();
                options.Filters.Add<ActionFilter>();
                options.Filters.Add<ExceptionFilter>();
            }).AddNewtonsoftJson();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "MsSystem API"
                });

                //Determine base path for the application.  
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                //Set the comments path for the swagger json and ui.  
                var xmlPath = Path.Combine(basePath, "ProjectNameTemplate.Host.xml");
                options.IncludeXmlComments(xmlPath);

                options.OperationFilter<OperationFilter>();
            });

            //TODO  这里修改成需要映射的类库集合
            var autpTypes = Assembly.Load("ProjectNameTemplate.Host").GetTypes().ToList();
            //autpTypes.AddRange(Assembly.Load("ProjectNameTemplate.Host.Contract").GetTypes().ToList());
            autpTypes.AddRange(Assembly.Load("ProjectNameTemplate.Core").GetTypes().ToList());
            autpTypes.AddRange(Assembly.Load("ProjectNameTemplate.Application").GetTypes().ToList());
            AutoMapperModule.Initialize(autpTypes);

            //return new AutofacServiceProvider(InitContainerBuilder(services));//第三方IOC接管 core内置DI容器 
        }

        /// <summary>
        /// 使用Autofac注入
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var module = ModuleManager.Create<HostModule>();
            var tyeps = typeof(Startup).Assembly
                .GetTypes()
                .Where(t => t.IsClass && t.Name.EndsWith("Controller"))
                .ToArray();
            //注入MVC控制器（配置属性注入）
            builder.RegisterTypes(tyeps).PropertiesAutowired();
            builder.RegisterLogger();//https://github.com/nblumhardt/autofac-serilog-integration            
            module.ExternalBuilderInitialize(builder);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            //https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.0
            app.UseCors("AllowSameDomain");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MsSystem API V1");
            });

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{action}/{controller}/{id?}",
            //        defaults: new { controller = "Home", action = "Index" });
            //});

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// 初始化 注入容器
        /// </summary>
        private IContainer InitContainerBuilder(IServiceCollection services)
        {
            services.AddDirectoryBrowser();
            var module = ModuleManager.Create<HostModule>();
            var builder = module.ContainerBuilder;
            var tyeps = typeof(Startup).Assembly
                .GetTypes()
                .Where(t => t.IsClass && t.Name.EndsWith("Controller"))
                .ToArray();
            //注入MVC控制器（配置属性注入）
            builder.RegisterTypes(tyeps).PropertiesAutowired();
            builder.RegisterLogger();//https://github.com/nblumhardt/autofac-serilog-integration
            builder.Populate(services);
            module.Initialize();
            return module.Container;
        }

        /// <summary>
        /// 日志配置
        /// </summary>      
        private void LogConfig()
        {
            //nuget导入
            //Serilog.Extensions.Logging
            //Serilog.Sinks.RollingFile
            //Serilog.Sinks.Async
            var basePath = "./File/logs";
            var fileSize = 1024 * 1024 * 100;//100M
            var fileCount = 5;
            Log.Logger = new LoggerConfiguration()
                                 .Enrich.FromLogContext()
                                 .MinimumLevel.Debug()
                                 .MinimumLevel.Override("System", LogEventLevel.Information)
                                 .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                                 .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Debug).WriteTo.Async(
                                     a => a.RollingFile(basePath + "/log-{Hour}-Debug.txt", fileSizeLimitBytes: fileSize, retainedFileCountLimit: fileCount)
                                 ))
                                 .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Information).WriteTo.Async(
                                     a => a.RollingFile(basePath + "/log-{Date}-Information.txt", fileSizeLimitBytes: fileSize, retainedFileCountLimit: fileCount)
                                 ))
                                 .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Warning).WriteTo.Async(
                                     a => a.RollingFile(basePath + "/log-{Date}-Warning.txt", fileSizeLimitBytes: fileSize, retainedFileCountLimit: fileCount)
                                 ))
                                 .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Error).WriteTo.Async(
                                     a => a.RollingFile(basePath + "/log-{Date}-Error.txt", fileSizeLimitBytes: fileSize, retainedFileCountLimit: fileCount)
                                 ))
                                 .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Fatal).WriteTo.Async(
                                     a => a.RollingFile(basePath + "/log-{Date}-Fatal.txt", fileSizeLimitBytes: fileSize, retainedFileCountLimit: fileCount)
                                 ))
                                 //所有情况
                                 .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => true)).WriteTo.Async(
                                     a =>
                                     {
                                         a.RollingFile(basePath + "/log-{Hour}-All.txt", fileSizeLimitBytes: fileSize, retainedFileCountLimit: fileCount);

                                         #region 存入ES
                                         //var url = ConfigurationManager.GetSection("ESURL");
                                         //var logIndexFormat = ConfigurationManager.GetSection("ESLogTypeName");
                                         //a.Elasticsearch(new ElasticsearchSinkOptions(new Uri(url))
                                         //{
                                         //    IndexFormat = logIndexFormat,//标记（可不同项目做不同标记）。注意：只能是小写或数字
                                         //    AutoRegisterTemplate = true,
                                         //    BufferBaseFilename = basePath + "/buffer",  //ES写入失败缓冲区
                                         //    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
                                         //}); 
                                         #endregion
                                     }
                                 )
                                .CreateLogger();
        }
    }
}
