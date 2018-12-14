using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using DotNet.Demo.IServices;
using DotNet.Demo.Models;
using DotNet.Demo.WebAPI.Controllers;
using DotNet.Standard.NSmart.Utilities;
using DotNet.Standard.Common.Utilities;

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
    }
}