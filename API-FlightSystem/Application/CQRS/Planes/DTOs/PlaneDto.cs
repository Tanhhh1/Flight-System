using Domain.Enums;

namespace Application.CQRS.Planes.DTOs
{
    public class PlaneDto
    {
        public int PlaneId { get; set; }
        public string PlaneModel { get; set; } = string.Empty;
        public int AirlineId { get; set; }
        public string AirlineName { get; set; } = string.Empty;
        public FlightStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
