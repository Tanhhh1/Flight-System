using Domain.Common;

namespace Domain.Entities
{
    public class FlightService : BaseEntity
    {
        public int FlightId { get; set; }
        public int ServiceId { get; set; }
        public Flight Flight { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }
}
