using Domain.Enums;

namespace Application.CQRS.Bookings.DTOs
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string TripType { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public List<BookingDetailDto> BookingDetails { get; set; } = [];
    }

    public class BookingDetailDto
    {
        public int FlightId { get; set; }
        public decimal UnitPrice { get; set; }
        public PassengerDto Passenger { get; set; } = null!;
    }

    public class PassengerDto
    {
        public int TypeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Country { get; set; } = string.Empty;
    }
}