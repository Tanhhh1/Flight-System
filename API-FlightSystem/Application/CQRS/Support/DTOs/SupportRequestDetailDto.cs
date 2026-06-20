namespace Application.CQRS.Support.DTOs
{
    public class SupportRequestDetailDto
    {
        public int RequestId { get; set; }
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public NewFlightInfoDto? NewFlight { get; set; }
    }

    public class NewFlightInfoDto
    {
        public int FlightId { get; set; }
        public string OriginAirport { get; set; } = string.Empty;
        public string OriginAirportName { get; set; } = string.Empty;
        public string DestinationAirport { get; set; } = string.Empty;
        public string DestinationAirportName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
