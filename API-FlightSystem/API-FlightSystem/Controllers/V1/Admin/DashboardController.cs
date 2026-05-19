using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Statistics.DTOs;
using Application.CQRS.Statistics.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Admin
{
    public class DashboardController : AdminApiController
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<StatisticDtos>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<StatisticDtos>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetDashboardQuery(), cancellationToken);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
