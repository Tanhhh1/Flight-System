using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Support.Commands.Create;
using Application.CQRS.Support.DTOs;
using Application.CQRS.Support.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Client
{
    [Authorize(Roles = "user")]
    public class SupportRequestController : ApiController
    {
        private readonly IMediator _mediator;

        public SupportRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("my-request")]
        [ProducesResponseType(typeof(ApiResult<PageList<SupportRequestDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<SupportRequestDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMySupportRequests([FromQuery] GetMySupportRequestsQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<SupportRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<SupportRequestDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSupportRequest([FromBody] CreateSupportRequestCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}