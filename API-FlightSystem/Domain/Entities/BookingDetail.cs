using Domain.Common;

namespace Domain.Entities
{
    public class BookingDetail : BaseEntity
    {
        public int BookingDetailId { get; set; }
        public int BookingId { get; set; }
        public int BookingFlightId { get; set; }
        public int PassengerId { get; set; }
        public int? FlightSeatId { get; set; }
        public decimal UnitPrice { get; set; }
        public Booking Booking { get; set; } = null!;
        public Flight Flight { get; set; } = null!;
        public Passenger Passenger { get; set; } = null!;
        public FlightSeat? FlightSeat { get; set; } = null!;
    }
}
