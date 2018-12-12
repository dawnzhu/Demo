using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using DotNet.Core.Demo.IServices;
using DotNet.Core.Demo.Models;
using DotNet.Core.Demo.WebAPI.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //依赖注入的配置
            services.AddMvc(option =>
            {
                option.Filters.Add(typeof(CustomerActionFilterAttribute));
                option.Filters.Add(typeof(CustomerExceptionFilterAttribute));
            }).AddJsonOptions(option =>
            {
                option.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                option.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
            }).AddControllersAsServices().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            var builder = new ContainerBuilder();
            builder.Populate(services);
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
            var container = builder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
        }
    }
}
