namespace Application.CQRS.Flights.DTOs
{
    public class FlightDetailSegmentDto
    {
        public int SegmentOrder { get; set; }
        public string OriginAirportName { get; set; } = string.Empty;
        public string OriginAirportCode { get; set; } = string.Empty;
        public string DestinationAirportName { get; set; } = string.Empty;
        public string DestinationAirportCode { get; set; } = string.Empty;
        public int FlightDuration { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
