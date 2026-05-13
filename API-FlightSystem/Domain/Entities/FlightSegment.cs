using Domain.Common;

namespace Domain.Entities
{
    public class FlightSegment : BaseEntity
    {
        public int SegmentId { get; set; }  
        public int FlightId { get; set; }
        public int RouteId { get; set; }
        public int FlightDuration { get; set; }
        public DateTime DepartureTime { get; set; } 
        public DateTime ArrivalTime { get; set; }
        public int SegmentOrder { get; set; }
        public Flight Flight { get; set; } = null!;
        public Route Route { get; set; } = null!;
    }
}
