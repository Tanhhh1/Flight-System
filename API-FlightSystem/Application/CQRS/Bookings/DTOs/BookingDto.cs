using Domain.Enums;

namespace Application.CQRS.Bookings.DTOs
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string UserFullname { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string TripType { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public List<BookingDetailDto> BookingDetails { get; set; } = [];
    }
}