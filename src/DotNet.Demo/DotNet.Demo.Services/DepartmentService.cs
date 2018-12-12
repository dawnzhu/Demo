using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotNet.Demo.IServices;
using DotNet.Demo.Models;
using DotNet.Demo.Utilities;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NSmart.Utilities;

namespace DotNet.Demo.Services
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
