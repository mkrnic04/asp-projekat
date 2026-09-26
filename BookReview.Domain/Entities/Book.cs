using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Domain
{
    public class Book : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ISBN { get; set; } = string.Empty;

        public int PublishedYear { get; set; }

        public string? CoverImagePath { get; set; }

        public virtual HashSet<Review> Reviews { get; set; } = new();

        public virtual HashSet<Favorite> Favorites { get; set; } = new();

        public virtual HashSet<BookAuthor> BookAuthors { get; set; } = new();

        public virtual HashSet<BookCategory> BookCategories { get; set; } = new();
    }
}
