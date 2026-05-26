using Domain.Enums;

namespace Application.CQRS.Bookings.DTOs
{
    public class BookingByIdDto
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string TripType { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public List<BookingFlightDto> Flights { get; set; } = [];
    }

    public class BookingFlightDto
    {
        public int FlightId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string FlightStatus { get; set; } = string.Empty;
        public string AirlineName { get; set; } = string.Empty;
        public string PlaneModel { get; set; } = string.Empty;
        public string OriginAirport { get; set; } = string.Empty;
        public string DestinationAirport { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public List<PassengerDto> Passengers { get; set; } = [];
    }
}