using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Bookings.Commands.Booking
{
    public class CreateBookingCommand : IRequest<ApiResult<BookingDto>>
    {
        public int ClassId { get; set; }
        public TripType TripType { get; set; }
        public List<int> FlightIds { get; set; } = [];
        public List<PassengerDto> Passengers { get; set; } = [];
    }
}
