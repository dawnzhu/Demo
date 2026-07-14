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
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Globalization;

namespace DotNet.Core.Demo.WebAPI.Filters
{
    public class CustomerActionFilterAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = (BaseController)context.Controller;
            var strRequestJsons = (List<string>)context.HttpContext.Items["_request_jsons"];
            controller.RequestParam = strRequestJsons.ToRequestParam<RequestParamInfo>();
            foreach (var property in controller.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(obj => typeof(IBaseService).IsAssignableFrom(obj.PropertyType)))
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

    public class RequestMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var strRequestJsons = new List<string> { request.QueryString.Value.ToDictionary().ToJsonString() };
            if (request.ContentType != null && request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                request.EnableBuffering();
                var body = await new StreamReader(request.Body).ReadToEndAsync();
                if (!string.IsNullOrEmpty(body) && body.StartsWith("{") && body.EndsWith("}"))
                {
                    strRequestJsons.Add(body);
                }
                request.Body.Position = 0;
            }
            context.Items.Add("_request_jsons", strRequestJsons);
            await _next(context);
        }
    }
}