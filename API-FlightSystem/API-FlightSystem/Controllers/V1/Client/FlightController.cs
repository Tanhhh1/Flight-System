using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.CQRS.Flights.Queries.GetById;
using Application.CQRS.Flights.Queries.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Client
{
    public class FlightController : ApiController
    {
        private readonly IMediator _mediator;
        public FlightController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<FlightSearchDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<FlightSearchDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromQuery] SearchFlightQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<FlightDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<FlightDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetByFlightIdQuery { FlightId = id });
            if (!result.Succeeded)
                return NotFound(result);
            return Ok(result);
        }
    }
}
