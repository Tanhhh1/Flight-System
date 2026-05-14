namespace Application.CQRS.Flights.DTOs
{
    public class FlightDto
    {
        public int FlightId { get; set; }
        public int PlaneId { get; set; }
        public int RouteId { get; set; }
        public bool IsRefund { get; set; }
        public bool IsChange { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<FlightSegmentDto> Segments { get; set; } = [];
        public List<FlightSeatPriceDto> SeatPrices { get; set; } = [];
        public List<FlightServiceDto> Services { get; set; } = [];
    }
}
