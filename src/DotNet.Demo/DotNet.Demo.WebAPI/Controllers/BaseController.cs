using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using DotNet.Demo.IServices;
using DotNet.Demo.Models;

namespace DotNet.Demo.WebAPI.Controllers
{
    public class BaseController<TS, TM> : BaseController
        where TS : IBaseService<TM>
        where TM : BaseInfo, new ()
    {
        public TS Service { get; set; }

        /// <summary>
        /// 管理列表
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        public virtual async Task<ResultInfo<IList<TM>>> Patch()
        {
            return await Service.GetList();
        }

        /// <summary>
        /// 读取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<ResultInfo<IList<TM>>> Get()
        {
            return await Service.GetList();
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<ResultInfo<TM>> Get(int id)
        {
            return await  Service.GetModel(id);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultInfo<TM>> Post(TM model)
        {
            return await Service.Add(model);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResultInfo<TM>> Put(int id, TM model)
        {
            model.Id = id;
            return await Service.Update(model);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        public virtual async Task<ResultInfo<IList<TM>>> Delete(int[] ids)
        {
            var result = await Service.Delete(ids);
            return result;
        }
    }

    public class BaseController : ApiController, IApiBase
    {
        public RequestParamInfo RequestParam { get; set; }
    }
}