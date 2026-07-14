using System.Web.Http;
using DotNet.Demo.WebAPI.Filters;
using DotNet.Standard.NSmart;
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
                //ContractResolver = new CamelCasePropertyNamesContractResolver(), //属性小驼峰命名
                ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }, //属性蛇形命名
                NullValueHandling = NullValueHandling.Ignore, //忽略null属性
                DateFormatString = "yyyy-MM-dd HH:mm:ss.fff", //日期格式
                Converters = new JsonConverter[]
                {
                    new DoModelConverter(), //DoModel对象转换
                    new StringEnumConverter() //枚举转字符串
                }
            };
        }
    }
}
