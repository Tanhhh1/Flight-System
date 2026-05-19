using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Bookings.Commands.Booking;
using Application.CQRS.Bookings.DTOs;
using Application.CQRS.Bookings.Queries.Transaction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Client
{
    [Authorize(Roles = "user")]
    public class BookingController : ApiController
    {
        private readonly IMediator _mediator;
        public BookingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<BookingDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateBookingCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("my-bookings")]
        [ProducesResponseType(typeof(ApiResult<PageList<BookingDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<BookingDto>>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyBookings([FromQuery] GetUserBookingQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Succeeded)
                return Unauthorized(result);
            return Ok(result);
        }
    }
}
