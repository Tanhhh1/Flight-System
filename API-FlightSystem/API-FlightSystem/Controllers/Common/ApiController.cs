using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.Common
{
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class ApiController : ControllerBase
    {

    }
}
