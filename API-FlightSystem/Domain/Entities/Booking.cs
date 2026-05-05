using Domain.Common;
using Domain.Enums;
using Domain.Identity;

namespace Domain.Entities
{
    public class Booking : BaseEntity
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int FlightId { get; set; }
        public DateTime BookingDate { get; set; }
        public int TripType { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public Flight Flight { get; set; } = null!;
        public User User { get; set; } = null!;
        public ICollection<SupportRequest> SupportRequests { get; set; } = new List<SupportRequest>();
        public ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
