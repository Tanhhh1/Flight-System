namespace Application.CQRS.Payments.DTOs
{
    public class ProcessCallbackDto
    {
        public bool IsSuccess { get; set; }
        public int BookingId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
