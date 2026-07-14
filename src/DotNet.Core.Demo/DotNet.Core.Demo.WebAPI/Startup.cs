using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using DotNet.Core.Demo.IServices;
using DotNet.Core.Demo.Models;
using DotNet.Core.Demo.WebAPI.Filters;
using DotNet.Standard.NSmart;
using DotNet.Standard.NSmart.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace DotNet.Core.Demo.WebAPI
{
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
            services.AddNSmart(option =>
            {
                option.Use(Configuration);
            }).AddMvc(option =>
            {
                option.Filters.Add(typeof(CustomerActionFilterAttribute));
                option.Filters.Add(typeof(CustomerExceptionFilterAttribute));
            }).AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.UseNSmart();
                /*
                //忽略null属性
                option.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                //日期格式
                option.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
                //属性蛇形命名
                option.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
                //DoModel对象转换
                option.SerializerSettings.Converters.Add(new DoModelConverter());
                //枚举转字符串
                option.SerializerSettings.Converters.Add(new StringEnumConverter());*/
            });
            //services.AddControllersWithViews().AddControllersAsServices();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //注册拦截器类
            builder.RegisterType<ServiceInterceptor>();
            builder.RegisterAssemblyTypes(Assembly.Load("DotNet.Core.Demo.Services"))
                .Where(type => typeof(IBaseService).IsAssignableFrom(type) && !type.IsAbstract)
                .AsImplementedInterfaces() //根据名称约定，实现服务接口和服务实现的依赖
                .InstancePerLifetimeScope() //生命周期
                .PropertiesAutowired()
                .EnableInterfaceInterceptors() //启用拦截器
                .InterceptedBy(typeof(ServiceInterceptor));
            builder.RegisterAssemblyTypes(Assembly.GetEntryAssembly()).PropertiesAutowired();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthorization();
            app.UseMiddleware<RequestMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
