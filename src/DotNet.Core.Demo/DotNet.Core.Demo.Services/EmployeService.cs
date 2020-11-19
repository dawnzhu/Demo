using System;
using System.Collections.Generic;
using System.Reflection;
using DotNet.Core.Demo.IServices;
using DotNet.Core.Demo.Models;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NSmart.Utilities;

namespace DotNet.Core.Demo.Services
{
    public class EmployeService : BaseService<EmployeInfo>, IEmployeService
    {
        protected override void GetList(ref IObQueryable<EmployeInfo> queryable, IDictionary<string, object> requestParams, IDictionary<string, object> requestGroupParams,
            IDictionary<string, string> requestSorts)
        {
            base.GetList(ref queryable, requestParams, requestGroupParams, requestSorts);
            queryable = MethodBase.GetCurrentMethod().CreateQueryable(queryable, requestParams, requestGroupParams, requestSorts);
        }

        protected override void OnAdding(EmployeInfo model, ref IObQueryable<EmployeInfo> queryable)
        {
            base.OnAdding(model, ref queryable);
            model.CreateTime = DateTime.Now;
        }

        protected override void OnUpdating(EmployeInfo model, ref IObQueryable<EmployeInfo> queryable)
        {
            base.OnUpdating(model, ref queryable);
            queryable.Join();
        }
    }
}
