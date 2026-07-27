namespace Application.Services
{
    public class BookingConfirmationEmailDto
    {
        public string ToEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string BookingCode { get; set; } = string.Empty;
        public string TripType { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public List<FlightEmailDto> Flights { get; set; } = new();
    }

    public class FlightEmailDto
    {
        public string OriginAirport { get; set; } = string.Empty;
        public string OriginAirportName { get; set; } = string.Empty;
        public string DestinationAirport { get; set; } = string.Empty;
        public string DestinationAirportName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string AirlineName { get; set; } = string.Empty;
        public string PlaneModel { get; set; } = string.Empty;
        public List<PassengerEmailDto> Passengers { get; set; } = new();
    }

    public class PassengerEmailDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }

    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(BookingConfirmationEmailDto dto);
    }
}