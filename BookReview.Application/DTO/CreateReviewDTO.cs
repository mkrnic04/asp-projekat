using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Application.DTO
{
    public class CreateReviewDTO
    {
        public int BookId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public int Rating { get; set; }
    }
}
