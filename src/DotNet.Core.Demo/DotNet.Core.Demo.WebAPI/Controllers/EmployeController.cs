using DawnZhu.NBubble.Models.SystemModels;
using DotNet.Core.Demo.IServices;
using DotNet.Core.Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Core.Demo.WebAPI.Controllers
{
    public class EmployeController : BaseController<IEmployeService, EmployeInfo>
    {
        [HttpPost]
        public async Task<ResultInfo<object>> Index(UserInfo model)
        {
            var userName = model.UserName;
            var PassWord = model.PassWord;
            return await Task.Run(() => new ResultInfo<object>());
        }
     }
}