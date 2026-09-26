using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Domain
{
    public class Favorite : BaseEntity
    {
        public int UserId { get; set; }

        public int BookId { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual User User { get; set; } = null!;

        public virtual Book Book { get; set; } = null!;
    }
}