using Domain.Enums;

namespace Application.CQRS.Bookings.DTOs
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public int UserdId { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public int ClasseId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string TripType { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public List<BookingDetailDto> BookingDetails { get; set; } = [];
    }
}