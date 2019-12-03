using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using DotNet.Demo.IServices;
using DotNet.Demo.Models;
using DotNet.Demo.WebAPI.Controllers;
using DotNet.Standard.NSmart.Utilities;
using DotNet.Standard.Common.Utilities;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NSmart;

namespace DotNet.Demo.WebAPI.Filters
{
    public class CustomerActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            base.OnActionExecuting(context);
            var controller = (BaseController)context.ControllerContext.Controller;
            try
            {
                #region Request数据处理

                var request = context.Request;
                var strRequestJsons = new List<string> { request.RequestUri.Query.ToDictionary().ToJsonString() };
                if (request.Content.Headers.ContentType != null
                    && request.Content.Headers.ContentType.MediaType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
                {
                    var postData = System.Web.HttpContext.Current.Request.InputStream;
                    var reader = new StreamReader(postData);
                    var postContent = reader.ReadToEnd();
                    postData.Position = 0;
                    if (!string.IsNullOrEmpty(postContent) && postContent.StartsWith("{") && postContent.EndsWith("}"))
                    {
                        strRequestJsons.Add(postContent);
                    }
                }
                controller.RequestParam = strRequestJsons.ToRequestParam<RequestParamInfo>();

                //ToActionProxyArguments(controller.RequestParam.Params, context.ActionArguments);

                #endregion
            }
            catch (Exception er)
            {
                LogUtil.WriteLog(er);
            }

            foreach (var property in controller.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(obj => obj.Name.EndsWith("Service")))
            {
                var obj = property.GetValue(controller);
                if (obj == null) continue;
                var baseService = obj as IBaseService;
                baseService?.Initialize(controller);
            }
        }

        private static Dictionary<string, object> ToActionProxyArguments(IDictionary<string, object> requestParams, Dictionary<string, object> actionArguments)
        {
            foreach (var key in actionArguments.Keys.ToList())
            {
                var value = actionArguments[key];
                var valueType = value.GetType();
                if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var subRequestParams = requestParams.First(o => string.Equals(o.Key, key, StringComparison.OrdinalIgnoreCase)).Value.ToJsonString().ToObject<IList<Dictionary<string, object>>>();
                    foreach (var subRequestParam in subRequestParams)
                    {
                        var subObj = Activator.CreateInstance(valueType.GenericTypeArguments.First());
                        valueType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public)?.Invoke(value, new [] { ToActionProxyArgument(subObj, subRequestParam) });
                    }
                    actionArguments[key] = value;
                }
                else
                {
                    actionArguments[key] = ToActionProxyArgument(value, requestParams);
                }
            }
            return actionArguments;
        }

        private static object ToActionProxyArgument(object value, IDictionary<string, object>  requestParams)
        {
            if (!(value is DoModelBase)) return value;
            value = typeof(ObModel).GetMethod("Of", BindingFlags.Static | BindingFlags.Public)
                ?.MakeGenericMethod(value.GetType()).Invoke(null, new [] { value });
            if (value == null) return null;
            foreach (var param in requestParams)
            {
                var property = value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(o => string.Equals(o.Name, param.Key, StringComparison.OrdinalIgnoreCase));
                if (property == null) continue;
                if (property.PropertyType.IsSystem())
                {
                    property.SetValue(value, param.Value);
                }
                else
                {
                    var subRequestParams = param.Value is Newtonsoft.Json.Linq.JArray 
                        ? new Dictionary<string, object> {{param.Key, param.Value}} 
                        : param.Value.ToJsonString().ToObject<Dictionary<string, object>>();
                    var subObj = ToActionProxyArguments(subRequestParams, new Dictionary<string, object> {{property.Name, Activator.CreateInstance(property.PropertyType)}}).First();
                    property.SetValue(value, subObj.Value);
                }
            }
            return value;
        }
    }
}