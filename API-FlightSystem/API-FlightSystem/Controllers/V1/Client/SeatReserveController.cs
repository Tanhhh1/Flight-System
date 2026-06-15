using API_FlightSystem.Controllers.Common;
using Application.CQRS.SeatReverse.Queries.GetSeatMap;
using Application.CQRS.SeatReverse.Queries.Verify;
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
        public async Task<IActionResult> VerifyBooking([FromQuery] string bookingCode)
        {
            var result = await _mediator.Send(new VerifyBookingQuery { BookingCode = bookingCode });
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpGet("seat-map")]
        public async Task<IActionResult> GetSeatMap([FromQuery] int flightId, [FromQuery] int bookingId)
        {
            var result = await _mediator.Send(new GetSeatMapQuery
            {
                FlightId = flightId,
                BookingId = bookingId
            });
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}