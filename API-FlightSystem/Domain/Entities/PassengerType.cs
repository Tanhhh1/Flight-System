using Domain.Common;

namespace Domain.Entities
{
    public class PassengerType : BaseEntity
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public decimal DiscountRate { get; set; }
        public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
    }
}
