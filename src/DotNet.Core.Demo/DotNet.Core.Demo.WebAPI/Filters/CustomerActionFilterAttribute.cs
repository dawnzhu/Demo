using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNet.Core.Demo.IServices;
using DotNet.Core.Demo.Models;
using DotNet.Core.Demo.WebAPI.Controllers;
using DotNet.Standard.NSmart.Utilities;
using DotNet.Standard.Common.Utilities;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNet.Core.Demo.WebAPI.Filters
{
    public class CustomerActionFilterAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = (BaseController)context.Controller;
            try
            {
                #region Request数据处理

                var request = context.HttpContext.Request;
                var strRequestJsons = new List<string> { request.QueryString.Value.ToDictionary().ToJsonString() };
                if (request.ContentType != null 
                    && request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
                {
                    var postData = request.Body;
                    var reader = new StreamReader(postData);
                    var postContent = reader.ReadToEnd();
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

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}