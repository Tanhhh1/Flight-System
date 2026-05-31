using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Bookings.Queries.GetAll
{
    public class GetAllBookingQuery : IRequest<ApiResult<PageList<BookingListDto>>>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public TripType? TripType { get; set; }
        public int? ClassId { get; set; }
        public DateTime? BookingDate { get; set; }
        public BookingStatus? Status { get; set; }
    }
}
