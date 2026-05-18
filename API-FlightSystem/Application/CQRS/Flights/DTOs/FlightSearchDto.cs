namespace Application.CQRS.Flights.DTOs
{
    public class FlightSearchDto
    {
        public int FlightId { get; set; }
        public string AirlineName { get; set; } = string.Empty;
        public string PlaneName { get; set; } = string.Empty;
        public string OriginAirportCode { get; set; } = string.Empty;
        public string OriginCity { get; set; } = string.Empty;
        public string DestinationAirportCode { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int FlightDuration { get; set; }
        public int StopCount { get; set; }
        public bool IsRefund { get; set; }
        public bool IsChange { get; set; }
        public List<FlightSeatClassDto> SeatClasses { get; set; } = new();
    }
    public class FlightSeatClassDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
    }
}
