using DotNet.Demo.IServices;
using DotNet.Demo.Models;

namespace DotNet.Demo.WebAPI.Controllers
{
    public class EmployeController : BaseController<IEmployeService, EmployeInfo>
    {

    }
}