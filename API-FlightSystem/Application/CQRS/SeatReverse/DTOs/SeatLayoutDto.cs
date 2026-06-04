namespace Application.CQRS.SeatReverse.DTOs
{
    public class SeatLayoutDto
    {
        public int FlightSeatId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = null!;
        public int Status { get; set; }
        public int? LockedBy { get; set; }
        public DateTime? LockedUntil { get; set; }
    }
}
