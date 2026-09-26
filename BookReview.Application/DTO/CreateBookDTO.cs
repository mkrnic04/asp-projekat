using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Application.DTO
{
    public class CreateBookDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublishedYear { get; set; }

        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
    }
}
