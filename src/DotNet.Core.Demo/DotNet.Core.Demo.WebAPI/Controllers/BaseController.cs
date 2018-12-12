using System.Threading.Tasks;
using DotNet.Core.Demo.IServices;
using DotNet.Core.Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Core.Demo.WebAPI.Controllers
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
        public virtual async Task<ActionResult<dynamic>> Patch()
        {
            return await Service.GetList();
        }

        /// <summary>
        /// 读取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<ActionResult<dynamic>> Get()
        {
            return await Service.GetList();
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public virtual async Task<ActionResult<dynamic>> Get(int id)
        {
            return await  Service.GetModel(id);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<dynamic>> Post(TM model)
        {
            return await Service.Add(model);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<dynamic>> Put(int id, TM model)
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
        public virtual async Task<ActionResult<dynamic>> Delete(int[] ids)
        {
            var result = await Service.Delete(ids);
            return result;
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase, IApiBase
    {
        public RequestParamInfo RequestParam { get; set; }
    }
}