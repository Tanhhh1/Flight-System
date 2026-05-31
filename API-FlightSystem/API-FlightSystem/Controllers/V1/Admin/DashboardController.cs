using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Dashboard.DTOs;
using Application.CQRS.Dashboard.Queries.GetRevenue;
using Application.CQRS.Dashboard.Queries.GetSummary;
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

        [HttpGet("summary")]
        [ProducesResponseType(typeof(ApiResult<DashboardSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<DashboardSummaryDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _mediator.Send(new GetDashboardSummaryQuery());
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("revenue")]
        [ProducesResponseType(typeof(ApiResult<List<MonthlyRevenueDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<List<MonthlyRevenueDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRevenue([FromQuery] GetRevenueByYearQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}