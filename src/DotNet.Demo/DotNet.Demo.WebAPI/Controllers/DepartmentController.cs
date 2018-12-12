using DotNet.Demo.IServices;
using DotNet.Demo.Models;

namespace DotNet.Demo.WebAPI.Controllers
{
    public class DepartmentController : BaseController<IDepartmentService, DepartmentInfo>
    {
    }
}