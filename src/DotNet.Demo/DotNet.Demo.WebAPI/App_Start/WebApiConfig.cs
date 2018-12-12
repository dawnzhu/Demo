using System.Web.Http;
using DotNet.Demo.WebAPI.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace DotNet.Demo.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Filters.Add(new CustomerActionFilterAttribute());
            config.Filters.Add(new CustomerAuthorizationFilterAttribute());
            config.Filters.Add(new CustomerExceptionFilterAttribute());
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                //首字母小写
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore, //不显示值为null的属性
                Converters = new JsonConverter[]
                {
                    new IsoDateTimeConverter {DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff"} //日期格式化
                }
            };
        }
    }
}
