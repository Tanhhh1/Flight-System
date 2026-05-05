using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Airline : BaseEntity
    {
        public int AirlineId { get; set; }
        public string AirlineName { get; set; } = string.Empty;
        public FlightStatus Status { get; set; }
        public ICollection<Plane> Planes { get; set; } = new List<Plane>();
    }
}
