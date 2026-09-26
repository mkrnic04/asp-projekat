using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Domain
{
    public class Author : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Biography { get; set; } = string.Empty;

        public virtual HashSet<BookAuthor> BookAuthors { get; set; } = new();
    }
}