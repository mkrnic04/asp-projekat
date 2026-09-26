using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Application.DTO
{
    public class CreateFavoriteDTO
    {
        public int UserId { get; set; }

        public int BookId { get; set; }
    }
}
