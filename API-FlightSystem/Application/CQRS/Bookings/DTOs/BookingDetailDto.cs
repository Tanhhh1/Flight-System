namespace Application.CQRS.Bookings.DTOs
{
    public class BookingDetailDto
    {
        public int FlightId { get; set; }
        public decimal UnitPrice { get; set; }
        public PassengerDto Passenger { get; set; } = null!;
    }
}
