using Application.Common;
using Application.CQRS.Bookings.DTOs;
using MediatR;

namespace Application.CQRS.Bookings.Queries.GetById
{
    public class GetByBookingIdQuery : IRequest<ApiResult<BookingByIdDto>>
    {
        public int BookingId { get; set; }
    }
}
