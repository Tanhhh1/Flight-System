using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Application.CQRS.Bookings.Queries.GetAll;
using Application.CQRS.Bookings.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Admin
{
    public class BookingController : AdminApiController
    {
        private readonly IMediator _mediator;
        public BookingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PageList<BookingListDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<BookingListDto>>), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetAll([FromQuery] GetAllBookingQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<BookingDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetByBookingIdQuery { BookingId = id });
            if (!result.Succeeded)
                return NotFound(result);
            return Ok(result);
        }
    }
}
