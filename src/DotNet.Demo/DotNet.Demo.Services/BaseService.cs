using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DotNet.Demo.IServices;
using DotNet.Demo.Models;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NSmart;
using DotNet.Standard.NSmart.Utilities;

namespace DotNet.Demo.Services
{
    public class BaseService<TM, TT> : DoServiceBase<TM, TT>, IBaseService<TM>
        where TM: BaseInfo, new()
        where TT: BaseTerm, new()
    {
        private static readonly Dictionary<string, DoConfigDbs> DoConfigDbs = ConfigurationManager.GetSection("connectionConfigs") as Dictionary<string, DoConfigDbs>;
        public BaseService() : base(DoConfigDbs)
        {
            if (!DoParam.Initialized)
            {
                DoParam.Initialize(ConfigurationManager.GetSection("paramConfigs") as Dictionary<string, ParamConfig>);
            }
        }

        public RequestParamInfo RequestParam { get; protected set; }

        protected virtual ResultInfo OnAddingJudge(TM model, ref ObParameterBase param)
        {
            OnGlobalExecuting(ref param);
            return new ResultInfo();
        }

        protected virtual ResultInfo OnUpdateingJudge(TM model, ref ObParameterBase param)
        {
            OnGlobalExecuting(ref param);
            return new ResultInfo();
        }

        protected virtual ResultInfo OnDeleteingJudge(ref ObParameterBase param)
        {
            return new ResultInfo();
        }

        public new async Task<ResultInfo<TM>> Add(TM model)
        {
            //var ret = BaseAdd(model);
            ObParameterBase param = null;
            var ret = OnAddingJudge(model, ref param);
            var result = new ResultInfo<TM>(ret)
            {
                OperationCategory = OperationCategory.Add
            };
            if (!ret.IsSuccess())
                return result;
            model.Id = Convert.ToInt32(await AddAsync(model));
            result.Data = model;
            return result;
        }

        public async Task<ResultInfo<TM>> Update(TM model)
        {
            var param = Term.Id == model.Id;
            var ret = OnUpdateingJudge(model, ref param);
            var result = new ResultInfo<TM>(ret)
            {
                OperationCategory = OperationCategory.Mod
            };
            if (!ret.IsSuccess())
                return result;
            await UpdateAsync(model, param);
            result.Data = model;
            return result;
        }

        public async Task<ResultInfo<IList<TM>>> Delete(int[] ids)
        {
            var param = Term.Id.In(ids);
            var result = OnDeleteingJudge(ref param);
            var ret = new ResultInfo<IList<TM>>(result)
            {
                OperationCategory = OperationCategory.Del
            };
            if (!ret.IsSuccess())
                return ret;
            await DeleteAsync(param);
            return new ResultInfo<IList<TM>>
            {
                Data = ids.Select(id => new TM
                {
                    Id = id
                }).ToList()
            };
        }

        public async Task<ResultInfo<TM>> GetModel(int id)
        {
            var data = await GetModelAsync(Term.Id == id);
            return new ResultInfo<TM>
            {
                Data = data
            };
        }

        public async Task<ResultInfo<IList<TM>>> GetList()
        {
            var result = new ResultInfo<IList<TM>>();
            result.Data = await GetDataList(count => { result.Total = count; });
            return result;
        }

        public async Task<IList<TM>> GetDataList(Action<int> countAccessor)
        {
            return await GetListAsync(null, RequestParam.Params, null, RequestParam.Sorts, RequestParam.PageSize, RequestParam.PageIndex, countAccessor);
        }

        public void Initialize(IApiBase iApiBase)
        {
            foreach (var property in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(obj => obj.Name.EndsWith("Service")))
            {
                var obj = property.GetValue(this);
                if (obj == null) continue;
                var baseService = obj as IBaseService;
                baseService?.Initialize(iApiBase);
            }
            RequestParam = iApiBase.RequestParam;
        }
    }
}
