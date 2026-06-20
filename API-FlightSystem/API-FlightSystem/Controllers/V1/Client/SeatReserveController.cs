using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.SeatReserve.Commands.ConfirmSeat;
using Application.CQRS.SeatReserve.Commands.HoldSeat;
using Application.CQRS.SeatReserve.Commands.ReleaseSeat;
using Application.CQRS.SeatReserve.DTOs;
using Application.CQRS.SeatReserve.Queries.GetSeatMap;
using Application.CQRS.SeatReserve.Queries.Verify;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Client
{
    [Authorize(Roles = "user")]
    public class SeatReverseController : ApiController
    {
        private readonly IMediator _mediator;

        public SeatReverseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("verify")]
        [ProducesResponseType(typeof(ApiResult<VerifyBookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<VerifyBookingDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyBooking([FromQuery] string bookingCode)
        {
            var result = await _mediator.Send(new VerifyBookingQuery { BookingCode = bookingCode });
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("seat-map")]
        [ProducesResponseType(typeof(ApiResult<SeatMapDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<SeatMapDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeatMap([FromQuery] int flightId, [FromQuery] int bookingId)
        {
            var result = await _mediator.Send(new GetSeatMapQuery
            {
                FlightId = flightId,
                BookingId = bookingId
            });
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("hold")]
        [ProducesResponseType(typeof(ApiResult<HoldSeatDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<HoldSeatDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> HoldSeat([FromBody] HoldSeatCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("release")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReleaseSeat([FromBody] ReleaseSeatCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("confirm")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmSeats([FromBody] ConfirmSeatsCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}