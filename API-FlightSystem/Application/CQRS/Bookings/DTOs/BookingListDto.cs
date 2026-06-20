using Domain.Enums;

namespace Application.CQRS.Bookings.DTOs
{
    public class BookingListDto
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string TripType { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public string OriginAirport { get; set; } = string.Empty;
        public string DestinationAirport { get; set; } = string.Empty;
    }
}
