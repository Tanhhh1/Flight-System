using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class SupportRequest : BaseEntity
    {
        public int RequestId { get; set; }
        public int BookingId { get; set; }
        public RequestType RequestType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public SupportStatus Status { get; set; }
        public Booking Booking { get; set; } = null!;
    }
}
