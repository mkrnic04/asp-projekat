namespace BookReview.Application.DTO
{
    public class UpdateReviewDTO
    {
        public string Title { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public int Rating { get; set; }
    }
}