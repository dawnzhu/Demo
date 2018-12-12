using DotNet.Core.Demo.IServices;
using DotNet.Core.Demo.Models;

namespace DotNet.Core.Demo.WebAPI.Controllers
{
    public class EmployeController : BaseController<IEmployeService, EmployeInfo>
    {
    }
}