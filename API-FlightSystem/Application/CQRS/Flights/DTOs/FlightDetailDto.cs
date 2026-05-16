namespace Application.CQRS.Flights.DTOs
{
    public class FlightDetailDto
    {
        public int FlightId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int FlightDuration { get; set; }

        public string AirlineName { get; set; } = string.Empty;
        public string PlaneModel { get; set; } = string.Empty;

        public string OriginAirportName { get; set; } = string.Empty;
        public string OriginAirportCode { get; set; } = string.Empty;
        public string OriginCity { get; set; } = string.Empty;

        public string DestinationAirportName { get; set; } = string.Empty;
        public string DestinationAirportCode { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;

        public int StopCount { get; set; }
        public List<FlightSegmentDto> Stops { get; set; } = [];

        public List<FlightServiceDto> Services { get; set; } = [];

        public bool IsRefund { get; set; }
        public bool IsChange { get; set; }
    }
}
