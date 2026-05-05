using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Payment : BaseEntity
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public PaymentMethod Method { get; set; }
        public decimal TotalPrice { get; set; }
        public PaymentStatus Status { get; set; }
        public Booking Booking { get; set; } = null!;
    }
}
