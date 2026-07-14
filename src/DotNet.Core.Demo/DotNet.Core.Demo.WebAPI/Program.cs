//using System;
//using System.IO;
//using Autofac.Extensions.DependencyInjection;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Hosting;

//namespace DotNet.Core.Demo.WebAPI
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            CreateHostBuilder(args).Build().Run();
//        }

//        public static IHostBuilder CreateHostBuilder(string[] args) =>
//            Host.CreateDefaultBuilder(args)
//                .ConfigureWebHostDefaults(webBuilder =>
//                {
//                    var configuration = new ConfigurationBuilder()
//                        .SetBasePath(Environment.CurrentDirectory)
//                        .AddJsonFile("appsettings.json").Build();
//                    webBuilder.UseUrls(configuration["Host"]).UseStartup<Startup>();
//                }).UseServiceProviderFactory(new AutofacServiceProviderFactory());
//    }
//}
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using DotNet.Core.Demo.IServices;
using DotNet.Core.Demo.Models;
using DotNet.Core.Demo.WebAPI.Filters;
using DotNet.Standard.NSmart.Utilities;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(container => {
        //注册拦截器类
        container.RegisterType<ServiceInterceptor>();
        container.RegisterAssemblyTypes(Assembly.Load("DotNet.Core.Demo.Services"))
            .Where(type => typeof(IBaseService).IsAssignableFrom(type) && !type.IsAbstract)
            .AsImplementedInterfaces() //根据名称约定，实现服务接口和服务实现的依赖
            .InstancePerLifetimeScope() //生命周期
            .PropertiesAutowired()
            .EnableInterfaceInterceptors() //启用拦截器
            .InterceptedBy(typeof(ServiceInterceptor));
        container.RegisterAssemblyTypes(typeof(Program).Assembly).PropertiesAutowired();
    });
builder.Services
    .Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>())
    //.AddRedis(builder.Configuration)
    //.AddDistributedRedisCache(options =>
    //{
    //    options.Configuration = builder.Configuration.GetSection("CacheConfig:ConnectionString").Value;
    //    options.InstanceName = builder.Configuration.GetSection("CacheConfig:InstanceName").Value;
    //})
    .AddNSmart(option =>
    {
        option.Use(builder.Configuration);
    }).AddMvc(option =>
    {
        option.Filters.Add(typeof(CustomerActionFilterAttribute));
        option.Filters.Add(typeof(CustomerExceptionFilterAttribute));
    }).AddNewtonsoftJson(option =>
    {
        option.SerializerSettings.UseNSmart();
    });
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();
app.Urls.Add(app.Configuration["Host"]);
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<RequestMiddleware>();
app.MapControllers();
app.Run();
