namespace Application.CQRS.Flights.DTOs
{
    public class FlightListDto
    {
        public int FlightId { get; set; }
        public string PlaneModel { get; set; } = string.Empty;
        public string AirlineName { get; set; } = string.Empty;
        public string OriginAirport { get; set; } = string.Empty;
        public string DestinationAirport { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int FlightDuration { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsRefund { get; set; }
        public bool IsChange { get; set; }
        public List<FlightSeatPriceDto> SeatPrices { get; set; } = new();
    }
}
