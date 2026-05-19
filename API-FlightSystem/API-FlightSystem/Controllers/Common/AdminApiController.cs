using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.Common
{
    [ApiController]
    [Authorize(Roles = "admin, staff")]
    [Route("api/v{v:apiVersion}/admin/[controller]")]
    public class AdminApiController : ControllerBase
    {
    }
}
