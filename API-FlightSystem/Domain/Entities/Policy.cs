using Domain.Common;

namespace Domain.Entities
{
    public class Policy : BaseEntity
    {
        public int PolicyId { get; set; }
        public bool IsRefund { get; set; }
        public bool IsChange { get; set; }
        public ICollection<Flight> Flights { get; set; } = new List<Flight>();
    }
}
