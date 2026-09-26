using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Domain
{
    public class Comment : BaseEntity
    {
        public int ReviewId { get; set; }

        public int UserId { get; set; }

        public string Text { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public virtual Review Review { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}