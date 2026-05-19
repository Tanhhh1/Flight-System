using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Airlines.Commands.Create;
using Application.CQRS.Airlines.Commands.Delete;
using Application.CQRS.Airlines.Commands.Update;
using Application.CQRS.Airlines.DTOs;
using Application.CQRS.Airlines.Queries.GetAll;
using Application.CQRS.Airlines.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Admin
{
    public class AirlineController : AdminApiController
    {
        private readonly IMediator _mediator;
        public AirlineController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PageList<AirlineDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<AirlineDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllAirlineQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<AirlineDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<AirlineDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetByAirlineIdQuery { AirlineId = id });
            if (!result.Succeeded)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<AirlineDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResult<AirlineDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAirlineCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Result!.AirlineId }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<AirlineDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<AirlineDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAirlineCommand command)
        {
            if (id != command.AirlineId)
                return BadRequest(ApiResult<AirlineDto>.Failure(["Mã hãng bay không khớp"]));

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResult<AirlineDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<AirlineDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteAirlineCommand { AirlineId = id });
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
