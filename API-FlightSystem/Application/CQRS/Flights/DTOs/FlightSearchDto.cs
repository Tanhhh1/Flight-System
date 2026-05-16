namespace Application.CQRS.Flights.DTOs
{
    public class FlightSearchDto
    {
        public int FlightId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int FlightDuration { get; set; } 
        public string Status { get; set; } = string.Empty;

        public int PlaneId { get; set; }
        public string PlaneModel { get; set; } = string.Empty;
        public int AirlineId { get; set; }
        public string AirlineName { get; set; } = string.Empty;

        public int RouteId { get; set; }
        public int OriginAirportId { get; set; }
        public string OriginAirportCode { get; set; } = string.Empty;
        public string OriginCity { get; set; } = string.Empty;
        public int DestinationAirportId { get; set; }
        public string DestinationAirportCode { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;

        public bool IsRefund { get; set; }
        public bool IsChange { get; set; }

        public int StopCount { get; set; } 

        public decimal Price { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;

        public List<FlightServiceDto> Services { get; set; } = [];
    }
}
