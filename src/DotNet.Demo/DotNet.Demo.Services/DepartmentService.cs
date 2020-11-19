using System.Collections.Generic;
using System.Reflection;
using DotNet.Demo.IServices;
using DotNet.Demo.Models;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NSmart.Utilities;

namespace DotNet.Demo.Services
{
    public class DepartmentService : BaseService<DepartmentInfo, Department>, IDepartmentService
    {
        protected override void GetList(ref IObQueryable<DepartmentInfo, Department> queryable, IDictionary<string, object> requestParams, IDictionary<string, object> requestGroupParams,
            IDictionary<string, string> requestSorts)
        {
            base.GetList(ref queryable, requestParams, requestGroupParams, requestSorts);
            queryable = MethodBase.GetCurrentMethod().CreateQueryable(Term, queryable, requestParams, requestGroupParams, requestSorts);
        }
    }
}
