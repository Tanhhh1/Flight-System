using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Services.Commands.Create;
using Application.CQRS.Services.Commands.Delete;
using Application.CQRS.Services.Commands.Update;
using Application.CQRS.Services.DTOs;
using Application.CQRS.Services.Queries.GetAll;
using Application.CQRS.Services.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Admin
{
    public class ServiceController : AdminApiController
    {
        private readonly IMediator _mediator;
        public ServiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PageList<ServiceDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<ServiceDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllServiceQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<ServiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<ServiceDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetByServiceIdQuery { ServiceId = id });
            if (!result.Succeeded)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<ServiceDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResult<ServiceDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateServiceCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Result!.ServiceId }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<ServiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<ServiceDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateServiceCommand command)
        {
            if (id != command.ServiceId)
                return BadRequest(ApiResult<ServiceDto>.Failure(["Mã dịch vụ không khớp"]));

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResult<ServiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<ServiceDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteServiceCommand { ServiceId = id });
            if(!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
