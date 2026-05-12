using API_FlightBooking.Controllers.Common;
using Application.Common;
using Application.CQRS.Airports.Commands.Create;
using Application.CQRS.Airports.Commands.Delete;
using Application.CQRS.Airports.Commands.Update;
using Application.CQRS.Airports.DTOs;
using Application.CQRS.Airports.Queries.GetAll;
using Application.CQRS.Airports.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightBooking.Controllers.V1
{
    public class AirportController : ApiController
    {
        private readonly IMediator _mediator;

        public AirportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PageList<AirportDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<AirportDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllAirportQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<AirportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<AirportDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetByAirportIdQuery { AirportId = id });
            if (!result.Succeeded)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<AirportDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResult<AirportDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAirportCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Result!.AirportId }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<AirportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<AirportDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAirportCommand command)
        {
            if (id != command.AirportId)
                return BadRequest(ApiResult<AirportDto>.Failure(["Mã sân bay không khớp"]));

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResult<AirportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<AirportDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteAirportCommand { AirportId = id });
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
