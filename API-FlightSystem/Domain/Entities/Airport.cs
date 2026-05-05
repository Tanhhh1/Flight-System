using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Airport : BaseEntity
    {
        public int AirportId { get; set; }
        public string AirportCode { get; set; } = string.Empty;
        public string AirportName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public FlightStatus Status { get; set; }
        public ICollection<Route> OriginRoutes { get; set; } = new List<Route>();
        public ICollection<Route> DestinationRoutes { get; set; } = new List<Route>();
    }
}
