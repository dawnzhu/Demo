using System.Collections.Generic;
using System.Reflection;
using DotNet.Core.Demo.IServices;
using DotNet.Core.Demo.Models;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NSmart.Utilities;

namespace DotNet.Core.Demo.Services
{
    public class DepartmentService : BaseService<DepartmentInfo, Department>, IDepartmentService
    {
        protected override void GetList(ref ObParameterBase p, IDictionary<string, object> requestParams, ref ObParameterBase gp, IDictionary<string, object> requestGroupParams,
            ref IObSort s, IDictionary<string, string> requestSorts)
        {
            base.GetList(ref p, requestParams, ref gp, requestGroupParams, ref s, requestSorts);
            p = Term.CreateParameter(MethodBase.GetCurrentMethod(), p, requestParams);
            s = Term.CreateSort(MethodBase.GetCurrentMethod(), s, requestSorts);
        }
    }
}
