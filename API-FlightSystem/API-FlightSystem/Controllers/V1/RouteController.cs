using API_FlightBooking.Controllers.Common;
using Application.Common;
using Application.CQRS.Routes.Commands.Create;
using Application.CQRS.Routes.Commands.Delete;
using Application.CQRS.Routes.Commands.Update;
using Application.CQRS.Routes.DTOs;
using Application.CQRS.Routes.Queries.GetAll;
using Application.CQRS.Routes.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1
{
    public class RouteController : ApiController
    {
        private readonly IMediator _mediator;
        public RouteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PageList<RouteDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<RouteDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllRouteQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<PageList<RouteDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<RouteDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetByRouteIdQuery { RouteId = id });
            if (!result.Succeeded)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<PageList<RouteDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<RouteDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateRouteCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Result!.RouteId }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<RouteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<RouteDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRouteCommand command)
        {
            if (id != command.RouteId)
                return BadRequest(ApiResult<RouteDto>.Failure(["Mã tuyến bay không khớp"]));

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResult<RouteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<RouteDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteRouteCommand { RouteId = id });
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
