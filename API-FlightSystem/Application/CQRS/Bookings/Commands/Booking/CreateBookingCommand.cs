using Application.Common;
using Application.CQRS.Bookings.DTOs;
using MediatR;

namespace Application.CQRS.Bookings.Commands.Booking
{
    public class CreateBookingCommand : IRequest<ApiResult<BookingDto>>
    {
        public int ClassId { get; set; }
        public int TripType { get; set; }
        public List<BookingDetailDto> Details { get; set; } = [];
    }
}
