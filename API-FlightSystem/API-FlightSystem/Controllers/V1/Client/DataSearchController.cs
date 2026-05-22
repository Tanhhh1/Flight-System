using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.CQRS.Flights.Queries.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Client
{
    public class DataSearchController : ApiController
    {
        private readonly IMediator _mediator;
        public DataSearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<DataSearchDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<DataSearchDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMetadata([FromQuery] DataSearchQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
