using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using DotNet.Core.Demo.IServices;
using DotNet.Core.Demo.Models;
using DotNet.Core.Demo.WebAPI.Filters;
using DotNet.Standard.NSmart;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option =>
            {
                option.Filters.Add(typeof(CustomerActionFilterAttribute));
                option.Filters.Add(typeof(CustomerExceptionFilterAttribute));
            }).AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                option.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
                option.SerializerSettings.Converters.Add(new DoModelConverter());
            });
            services.AddControllersWithViews().AddControllersAsServices();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //ע����������
            builder.RegisterType<ServiceInterceptor>();
            builder.RegisterAssemblyTypes(Assembly.Load("DotNet.Core.Demo.Services"))
                .Where(type => typeof(IBaseService).IsAssignableFrom(type) && !type.IsAbstract)
                .AsImplementedInterfaces() //��������Լ����ʵ�ַ���ӿںͷ���ʵ�ֵ�����
                .InstancePerLifetimeScope() //��������
                .PropertiesAutowired()
                .EnableInterfaceInterceptors() //����������
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
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
