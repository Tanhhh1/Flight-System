using Application.Common;
using Application.CQRS.Bookings.DTOs;
using MediatR;

namespace Application.CQRS.Bookings.Queries.Transaction
{
    public class GetUserBookingQuery : IRequest<ApiResult<PageList<BookingDto>>>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
