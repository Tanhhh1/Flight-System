namespace Application.CQRS.Flights.DTOs
{
    public class FlightDetailDto
    {
        public int FlightId { get; set; }
        public int PlaneId { get; set; }
        public int RouteId { get; set; }
        public string PlaneName { get; set; } = string.Empty;
        public string AirlineName { get; set; } = string.Empty;
        public string OriginAirportCode { get; set; } = string.Empty;
        public string OriginCity { get; set; } = string.Empty;
        public string DestinationAirportCode { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int FlightDuration { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsRefund { get; set; }
        public bool IsChange { get; set; }

        public List<FlightDetailSegmentDto> Segments { get; set; } = [];
        public List<FlightDetailServiceDto> Services { get; set; } = [];
        public List<FlightDetailSeatPriceDto> SeatPrices { get; set; } = [];
    }

    public class FlightDetailSegmentDto
    {
        public int SegmentId { get; set; }
        public int RouteId { get; set; }
        public int StopOrder { get; set; }
        public string OriginAirportCode { get; set; } = string.Empty;
        public string OriginCity { get; set; } = string.Empty;
        public string DestinationAirportCode { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int FlightDuration { get; set; }
    }

    public class FlightDetailServiceDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
    }

    public class FlightDetailSeatPriceDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}