using Application.Common;
using Application.CQRS.Payments.Commands.InitiatePayment;
using Application.CQRS.Payments.Commands.ProcessPaymentCallback;
using Application.CQRS.Payments.Commands.RetryPayment;
using Application.CQRS.Payments.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IConfiguration _config;

        public PaymentsController(ISender sender, IConfiguration config)
        {
            _sender = sender;
            _config = config;
        }

        [HttpPost("initiate")]
        [ProducesResponseType(typeof(ApiResult<InitiateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<InitiateDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<InitiateDto>>> InitiatePayment([FromBody] InitiatePaymentCommand command)
        {
            command.ClientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            command.ReturnUrl = $"{Request.Scheme}://{Request.Host}/api/payments/callback/{command.Method}";

            var result = await _sender.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("callback/{method}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public async Task<IActionResult> PaymentCallback(string method, [FromQuery] Dictionary<string, string> parameters)
        {
            var command = new ProcessPaymentCallbackCommand
            {
                Method = method,
                Parameters = parameters
            };

            var result = await _sender.Send(command);
            var frontendBase = _config["ServerSetting:WithOrigins:0"];

            if (result.Succeeded && result.Result!.IsSuccess)
                return Redirect($"{frontendBase}/payment/success?bookingId={result.Result.BookingId}");

            return Redirect($"{frontendBase}/payment/failed?bookingId={result.Result?.BookingId}");
        }

        [HttpPost("retry")]
        [ProducesResponseType(typeof(ApiResult<InitiateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<InitiateDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<InitiateDto>>> RetryPayment([FromBody] RetryPaymentCommand command)
        {
            command.ClientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            command.ReturnUrl = $"{Request.Scheme}://{Request.Host}/api/payments/callback/{command.Method}";

            var result = await _sender.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}