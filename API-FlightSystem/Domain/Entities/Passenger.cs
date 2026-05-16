using Domain.Common;

namespace Domain.Entities
{
    public class Passenger : BaseEntity
    {
        public int PassengerId { get; set; }
        public int TypeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Country { get; set; } = string.Empty;
        public PassengerType PassengerType { get; set; } = null!;
        public ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    }
}
