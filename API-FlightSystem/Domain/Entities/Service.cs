using Domain.Common;

namespace Domain.Entities
{
    public class Service : BaseEntity
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public ICollection<FlightService> FlightServices { get; set; } = new List<FlightService>();
    }
}
