namespace Application.CQRS.Services.DTOs
{
    public class ServiceDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
         public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
