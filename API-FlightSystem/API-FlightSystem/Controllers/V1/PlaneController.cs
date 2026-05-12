using API_FlightBooking.Controllers.Common;
using Application.Common;
using Application.CQRS.Planes.Commands.Create;
using Application.CQRS.Planes.Commands.Delete;
using Application.CQRS.Planes.Commands.Update;
using Application.CQRS.Planes.DTOs;
using Application.CQRS.Planes.Queries.GetAll;
using Application.CQRS.Planes.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1
{
    public class PlaneController : ApiController
    {
        private readonly IMediator _mediator;

        public PlaneController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PageList<PlaneDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<PlaneDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPlaneQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<PlaneDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PlaneDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetByPlaneIdQuery { PlaneId = id });
            if (!result.Succeeded)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<PlaneDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResult<PlaneDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePlaneCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Result!.PlaneId }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<PlaneDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PlaneDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlaneCommand command)
        {
            if (id != command.PlaneId)
                return BadRequest(ApiResult<PlaneDto>.Failure(["Mã máy bay không khớp"]));

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResult<PlaneDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PlaneDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeletePlaneCommand { PlaneId = id });
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
