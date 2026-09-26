using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Domain
{
    public class BookCategory
    {
        public int BookId { get; set; }

        public int CategoryId { get; set; }

        public virtual Book Book { get; set; } = null!;

        public virtual Category Category { get; set; } = null!;
    }
}