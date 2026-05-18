namespace Application.CQRS.Bookings.DTOs
{
    public class PassengerDto
    {
        public int TypeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Country { get; set; } = string.Empty;
    }
}
