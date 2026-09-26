using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Domain
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public virtual HashSet<BookCategory> BookCategories { get; set; } = new();
    }
}