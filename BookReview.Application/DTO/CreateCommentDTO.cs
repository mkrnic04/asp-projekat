using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Application.DTO
{
    public class CreateCommentDTO
    {
        public int ReviewId { get; set; }

        public int UserId { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
