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
        public string OriginAirportName { get; set; } = string.Empty;
        public string DestinationAirport { get; set; } = string.Empty;
        public string DestinationAirportName { get; set; } = string.Empty;
        public int FlightDuration { get; set; }
        public List<BookingSegmentDto> Segments { get; set; } = [];
        public List<PassengerDetailDto> Passengers { get; set; } = [];
    }

    public class BookingSegmentDto
    {
        public int SegmentOrder { get; set; }
        public string OriginAirport { get; set; } = string.Empty;
        public string OriginAirportName { get; set; } = string.Empty;
        public string DestinationAirport { get; set; } = string.Empty;
        public string DestinationAirportName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int FlightDuration { get; set; }
    }

    public class PassengerDetailDto
    {
        public int TypeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Country { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }
}