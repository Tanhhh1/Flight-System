using API_FlightBooking.Controllers.Common;
using Application.Common;
using Application.CQRS.Bookings.Commands.Booking;
using Application.CQRS.Bookings.DTOs;
using Application.CQRS.Bookings.Queries.GetAll;
using Application.CQRS.Bookings.Queries.GetById;
using Application.CQRS.Bookings.Queries.Transaction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1
{
    public class BookingController : ApiController
    {
        private readonly IMediator _mediator;
        public BookingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PageList<BookingDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<BookingDto>>), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetAll([FromQuery] GetAllBookingQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<BookingDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int bookingId)
        {
            var result = await _mediator.Send(new GetByBookingIdQuery { BookingId = bookingId });
            if (!result.Succeeded)
                return NotFound(result);
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
