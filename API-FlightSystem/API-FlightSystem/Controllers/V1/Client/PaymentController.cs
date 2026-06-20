using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Payments.Commands.InitiatePayment;
using Application.CQRS.Payments.Commands.ProcessPaymentCallback;
using Application.CQRS.Payments.Commands.RetryPayment;
using Application.CQRS.Payments.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Client
{
    [Authorize]
    public class PaymentController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public PaymentController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        [HttpPost("initiate")]
        [ProducesResponseType(typeof(ApiResult<InitiateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<InitiateDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Initiate([FromBody] InitiatePaymentCommand command)
        {
            command.ReturnUrl = Url.Action(
                action: nameof(Callback),
                controller: "Payment",
                values: null,
                protocol: Request.Scheme,
                host: Request.Host.Value
            )!;
            command.ClientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            var result = await _mediator.Send(command);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("retry")]
        [ProducesResponseType(typeof(ApiResult<InitiateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<InitiateDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Retry([FromBody] RetryPaymentCommand command)
        {
            command.ReturnUrl = Url.Action(
                action: nameof(Callback),
                controller: "Payment",
                values: null,
                protocol: Request.Scheme,
                host: Request.Host.Value
            )!;
            command.ClientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            var result = await _mediator.Send(command);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("callback")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public async Task<IActionResult> Callback([FromQuery] Dictionary<string, string> parameters)
        {
            var feBaseUrl = _configuration.GetSection("ServerSetting:WithOrigins").Get<string[]>()?.FirstOrDefault();

            var command = new ProcessPaymentCallbackCommand
            {
                Method = "VNPay",
                Parameters = parameters
            };

            var result = await _mediator.Send(command);

            if (!result.Succeeded || !result.Result!.IsSuccess)
            {
                var bookingId = result.Result?.BookingId ?? 0;
                return Redirect($"{feBaseUrl}/payment?bookingId={bookingId}&status=failed");
            }

            return Redirect($"{feBaseUrl}/payment/success?bookingId={result.Result!.BookingId}");
        }
    }
}