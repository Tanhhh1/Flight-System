using Application.Common;
using Application.CQRS.Bookings.DTOs;
using MediatR;

namespace Application.CQRS.Bookings.Queries.Transaction
{
    public class GetUserBookingQuery : IRequest<ApiResult<PageList<BookingListDto>>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
