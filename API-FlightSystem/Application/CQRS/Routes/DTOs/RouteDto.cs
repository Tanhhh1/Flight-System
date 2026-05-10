using Domain.Enums;

namespace Application.CQRS.Routes.DTOs
{
    public class RouteDto
    {
        public int RouteId { get; set; }
        public int OriginAirportId { get; set; }
        public string OriginAirportCode { get; set; } = string.Empty;
        public int DestinationAirportId { get; set; }
        public string DestinationAirportCode { get; set; } = string.Empty;
        public int FlightDuration { get; set; }
        public FlightStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}