using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Bookings.Queries.GetAll
{
    public class GetAllBookingQuery : IRequest<ApiResult<PageList<BookingListDto>>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public TripType? TripType { get; set; }
        public int? ClassId { get; set; }
        public DateTime? BookingDate { get; set; }
        public BookingStatus? Status { get; set; }
    }
}
