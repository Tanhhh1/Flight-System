namespace Application.CQRS.Reviews.DTOs
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
    }
}
