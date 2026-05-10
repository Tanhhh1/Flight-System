using Domain.Enums;

namespace Application.CQRS.Airlines.DTOs
{
    public class AirlineDto
    {
        public int AirlineId { get; set; }
        public string AirlineName { get; set; } = string.Empty;
        public FlightStatus Status { get; set; } = FlightStatus.Active;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
