using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Profile.Commands.ChangePassword;
using Application.CQRS.Profile.Commands.Update;
using Application.CQRS.Profile.DTOs;
using Application.CQRS.Profile.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Client
{
    [Authorize]
    public class ProfileController : ApiController
    {
        private readonly IMediator _mediator;
        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<UserProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<UserProfileDto>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<UserProfileDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _mediator.Send(new GetByUserIdQuery());
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("Update-profile")]
        [ProducesResponseType(typeof(ApiResult<UserProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<UserProfileDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<UserProfileDto>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("Change-password")]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
