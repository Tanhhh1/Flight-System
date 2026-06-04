using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.SeatReverse.Commands.ConfirmSeat;
using Application.CQRS.SeatReverse.Commands.LockSeat;
using Application.CQRS.SeatReverse.Commands.UnlockSeat;
using Application.CQRS.SeatReverse.DTOs;
using Application.CQRS.SeatReverse.Queries.GetBookingPassengers;
using Application.CQRS.SeatReverse.Queries.GetFlightSeat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1
{
    [Authorize(Roles = "User")]
    public class SeatReserveController : ApiController
    {
        private readonly ISender _sender;

        public SeatReserveController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("layout/{flightId}")]
        [ProducesResponseType(typeof(ApiResult<List<SeatLayoutDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<List<SeatLayoutDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFlightSeatLayout(int flightId)
        {
            var result = await _sender.Send(new GetFlightSeatQuery { FlightId = flightId });
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("booking/{bookingCode}")]
        [ProducesResponseType(typeof(ApiResult<BookingPassengersDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<BookingPassengersDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBookingWithPassengers(string bookingCode)
        {
            var result = await _sender.Send(new GetBookingPassengersQuery { BookingCode = bookingCode });
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("lock")]
        [ProducesResponseType(typeof(ApiResult<LockSeatDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<LockSeatDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LockSeat([FromBody] LockSeatCommand command)
        {
            var result = await _sender.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("unlock")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UnlockSeat([FromBody] UnlockSeatCommand command)
        {
            var result = await _sender.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("confirm")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmSeats([FromBody] ConfirmSeatCommand command)
        {
            var result = await _sender.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}