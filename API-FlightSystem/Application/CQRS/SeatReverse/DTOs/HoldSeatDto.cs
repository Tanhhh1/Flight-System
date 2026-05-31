namespace Application.CQRS.SeatReverse.DTOs
{
    public class HoldSeatDto
    {
        public int FlightSeatId { get; set; }
        public int FlightId { get; set; }
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public int PassengerId { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public DateTime LockedUntil { get; set; }
    }
}
