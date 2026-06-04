namespace Application.CQRS.SeatReverse.DTOs
{
    public class BookingPassengersDto
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = null!;
        public int FlightId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = null!;
        public List<PassengerSeatDto> Passengers { get; set; } = new();
    }

    public class PassengerSeatDto
    {
        public int BookingDetailId { get; set; }
        public int PassengerId { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public int? FlightSeatId { get; set; }
        public string? SeatNumber { get; set; }
    }
}
