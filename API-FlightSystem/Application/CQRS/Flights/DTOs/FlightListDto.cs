namespace Application.CQRS.Flights.DTOs
{
    public class FlightListDto
    {
        public int FlightId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PlaneModel { get; set; } = string.Empty;
        public string AirlineName { get; set; } = string.Empty;
        public string OriginAirportCode { get; set; } = string.Empty;
        public string OriginCity { get; set; } = string.Empty;
        public string DestinationAirportCode { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;
        public int FlightDuration { get; set; }
        public bool IsRefund { get; set; }
        public bool IsChange { get; set; }
        public int StopCount { get; set; }
    }
}