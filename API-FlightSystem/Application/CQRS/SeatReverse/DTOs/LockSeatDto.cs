namespace Application.CQRS.SeatReverse.DTOs
{
    public class LockSeatDto
    {
        public int FlightSeatId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public DateTime LockedUntil { get; set; }
    }
}
