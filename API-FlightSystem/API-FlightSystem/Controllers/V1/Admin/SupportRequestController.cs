using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Support.Commands.Approve;
using Application.CQRS.Support.Commands.Reject;
using Application.CQRS.Support.DTOs;
using Application.CQRS.Support.Queries.GetAll;
using Application.CQRS.Support.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Admin
{
    public class SupportRequestController : AdminApiController
    {
        private readonly IMediator _mediator;

        public SupportRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PageList<SupportRequestDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<SupportRequestDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllSupportRequestsQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<SupportRequestDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<SupportRequestDetailDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _mediator.Send(new GetSupportRequestByIdQuery { RequestId = id });
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}/approve")]
        [ProducesResponseType(typeof(ApiResult<SupportRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<SupportRequestDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Approve([FromRoute] int id, [FromBody] ApproveSupportRequestCommand command)
        {
            command.RequestId = id;
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}/reject")]
        [ProducesResponseType(typeof(ApiResult<SupportRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<SupportRequestDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Reject([FromRoute] int id, [FromBody] RejectSupportRequestCommand command)
        {
            command.RequestId = id;
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
