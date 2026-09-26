using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace BookReview.Domain
{
    public class Review : BaseEntity
    {
        public int BookId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual Book Book { get; set; } = null!;

        public virtual User User { get; set; } = null!;

        public virtual HashSet<Comment> Comments { get; set; } = new();
    }
}
