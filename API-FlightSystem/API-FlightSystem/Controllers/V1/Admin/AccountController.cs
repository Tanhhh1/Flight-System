using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Accounts.Commands.Create;
using Application.CQRS.Accounts.Commands.Delete;
using Application.CQRS.Accounts.Commands.Update;
using Application.CQRS.Accounts.DTOs;
using Application.CQRS.Accounts.Queries.GetAll;
using Application.CQRS.Accounts.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Admin
{
    public class AccountController : AdminApiController
    {
        private readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PageList<AccountDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<AccountDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllAccountQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<AccountDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<AccountDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetByAccountIdQuery { UserId = id });
            if (!result.Succeeded)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResult<AccountDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResult<AccountDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAccountCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Result!.UserId }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResult<AccountDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<AccountDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAccountCommand command)
        {
            if (id != command.UserId)
                return BadRequest(ApiResult<AccountDto>.Failure("Mã tài khoản không khớp"));

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResult<AccountDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<AccountDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteAccountCommand { UserId = id });
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
