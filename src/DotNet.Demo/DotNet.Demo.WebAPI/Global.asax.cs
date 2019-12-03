using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Integration.WebApi;
using DotNet.Demo.IServices;
using DotNet.Demo.Models;

namespace DotNet.Demo.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();
            //注册拦截器类
            builder.RegisterType<ServiceInterceptor>();
            builder.RegisterAssemblyTypes(Assembly.Load("DotNet.Demo.Services"))
                .Where(type => typeof(IBaseService).IsAssignableFrom(type) && !type.IsAbstract)
                .AsImplementedInterfaces() //根据名称约定，实现服务接口和服务实现的依赖
                .InstancePerLifetimeScope() //生命周期
                .PropertiesAutowired()
                .EnableInterfaceInterceptors() //启用拦截器
                .InterceptedBy(typeof(ServiceInterceptor));
            /*builder.RegisterAssemblyTypes(Assembly.Load("JingEn.Ranebao.WebAPI"))
                .Where(type => typeof(IApiBase).IsAssignableFrom(type) && !type.IsAbstract)
                .EnableInterfaceInterceptors() //启用拦截器
                .InterceptedBy(typeof(TransactionInterceptor));*/
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()) //注入所有Controller
                .PropertiesAutowired(); //属性自动装配
            var container = builder.Build();
            var webApiResolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = webApiResolver;
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
