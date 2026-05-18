namespace Application.CQRS.Bookings.DTOs
{
    public class BookingDetailDto
    {
        public int BookingDetailId { get; set; }
        public int FlightId { get; set; }
        public int? FlightSeatId { get; set; }
        public decimal UnitPrice { get; set; }
        public PassengerDto Passenger { get; set; } = null!;
    }
}
