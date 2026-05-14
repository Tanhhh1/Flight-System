namespace Application.CQRS.Flights.DTOs
{
    public class FlightSegmentDto
    {
        public int SegmentId { get; set; }
        public int RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
