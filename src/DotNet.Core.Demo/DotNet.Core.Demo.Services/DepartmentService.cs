using System.Collections.Generic;
using System.Reflection;
using DotNet.Core.Demo.IServices;
using DotNet.Core.Demo.Models;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NSmart.Utilities;

namespace DotNet.Core.Demo.Services
{
    public class DepartmentService : BaseService<DepartmentInfo>, IDepartmentService
    {
        protected override void GetList(ref IObQueryable<DepartmentInfo> queryable, IDictionary<string, object> requestParams, IDictionary<string, object> requestGroupParams,
            IDictionary<string, string> requestSorts)
        {
            base.GetList(ref queryable, requestParams, requestGroupParams, requestSorts);
            queryable = MethodBase.GetCurrentMethod().CreateQueryable(queryable, requestParams, requestGroupParams, requestSorts);
        }
    }
}
