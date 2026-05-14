using API_FlightBooking.Controllers.Common;
using Application.Common;
using Application.CQRS.Flights.Commands.Create;
using Application.CQRS.Flights.Commands.Delete;
using Application.CQRS.Flights.Commands.Update;
using Application.CQRS.Flights.DTOs;
using Application.CQRS.Flights.Queries.GetAll;
using Application.CQRS.Flights.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1
{
    public class FlightController : ApiController
    {
        private readonly IMediator _mediator;
        public FlightController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<FlightDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<FlightDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllFlightQuery query)
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

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<FlightDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<FlightDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateFlightCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Result!.FlightId }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<FlightDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<FlightDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFlightCommand command)
        {
            if(id != command.FlightId)
                return BadRequest(ApiResult<FlightDto>.Failure(["Mã chuyến bay không khớp"]));

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResult<FlightDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<FlightDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteFlightCommand { FlightId = id });
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
