using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class FlightSeat : BaseEntity
    {
        public int FlightSeatId { get; set; }
        public int FlightId { get; set; }
        public int SeatId { get; set; }
        public SeatStatus Status { get; set; }
        public DateTime LockedUntil { get; set; }
        public int LockedBy { get; set; }
        public byte[] RowVersion { get; set; } = null!;
        public Flight Flight { get; set; } = null!;
        public SeatTemplate SeatTemplate { get; set; } = null!;
        public BookingDetail? BookingDetail { get; set; }
    }
}
