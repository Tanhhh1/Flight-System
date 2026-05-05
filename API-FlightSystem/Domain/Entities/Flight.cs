using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Flight : BaseEntity
    {
        public int FlightId { get; set; }
        public int PlaneId { get; set; }
        public int RouteId { get; set; }
        public int PolicyId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public FlightStatus Status { get; set; }
        public Plane Plane { get; set; } = null!;
        public Route Route { get; set; } = null!;
        public Policy Policy { get; set; } = null!;
        public ICollection<FlightService> FlightServices { get; set; } = new List<FlightService>();
        public ICollection<FlightSegment> FlightSegments { get; set; } = new List<FlightSegment>();
        public ICollection<FlightSeat> FlightSeats { get; set; } = new List<FlightSeat>();
        public ICollection<FlightSeatPrice> FlightSeatPrices { get; set; } = new List<FlightSeatPrice>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
