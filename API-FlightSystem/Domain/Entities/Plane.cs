using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Plane : BaseEntity
    {
        public int PlaneId { get; set; }
        public string PlaneModel { get; set; } = string.Empty;
        public int AirlineId { get; set; }
        public FlightStatus Status { get; set; }
        public Airline Airline { get; set; } = null!;
        public ICollection<Flight> Flights { get; set; } = new List<Flight>();
    }
}
