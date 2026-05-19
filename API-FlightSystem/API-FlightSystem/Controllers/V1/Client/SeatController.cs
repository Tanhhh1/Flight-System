using API_FlightSystem.Controllers.Common;
using Microsoft.AspNetCore.Authorization;

namespace API_FlightSystem.Controllers.V1.Client
{
    [Authorize(Roles = "user")]
    public class SeatController : ApiController
    {
    }
}
