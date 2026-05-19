using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Auth.Commands.RefreshToken;
using Application.CQRS.Auth.Commands.RevokeToken;
using Application.CQRS.Auth.Commands.SignIn;
using Application.CQRS.Auth.Commands.SignUp;
using Application.CQRS.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Client
{
    public class AuthController : ApiController
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("sign-up")]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("sign-in")]
        [ProducesResponseType(typeof(ApiResult<SignInDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignIn([FromBody] SignInCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("revoke")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("refresh")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResult<RefreshTokenDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}
