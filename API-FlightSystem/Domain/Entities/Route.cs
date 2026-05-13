using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Route : BaseEntity
    {
        public int RouteId { get; set; }
        public int OriginAirportId { get; set; }
        public int DestinationAirportId { get; set; }
        public int FlightDuration { get; set; }
        public FlightStatus Status { get; set; }
        public Airport OriginAirport { get; set; } = null!;
        public Airport DestinationAirport { get; set; } = null!;
        public ICollection<Flight> Flights { get; set; } = new List<Flight>();
        public ICollection<FlightSegment> FlightSegments { get; set; } = new List<FlightSegment>();
    }
}
