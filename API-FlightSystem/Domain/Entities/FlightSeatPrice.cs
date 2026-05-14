using Domain.Common;

namespace Domain.Entities
{
    public class FlightSeatPrice : BaseEntity
    {
        public int FlightId { get; set; }
        public int ClassId { get; set; }
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public Flight Flight { get; set; } = null!;
        public SeatClass SeatClass { get; set; } = null!;
    }
}
