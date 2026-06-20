using Domain.Enums;

namespace Application.CQRS.SeatReserve.DTOs
{
    public class VerifyBookingDto
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public TripType TripType { get; set; }
        public List<VerifyBookingFlightDto> Flights { get; set; } = [];
    }

    public class VerifyBookingFlightDto
    {
        public int FlightId { get; set; }
        public string OriginCode { get; set; } = string.Empty;
        public string DestinationCode { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public List<BookingPassengerDto> Passengers { get; set; } = [];
    }

    public class BookingPassengerDto
    {
        public int BookingDetailId { get; set; }
        public int PassengerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int? FlightSeatId { get; set; }
        public string? SeatNumber { get; set; }
    }
}
