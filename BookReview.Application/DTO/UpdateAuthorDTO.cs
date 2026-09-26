using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Application.DTO
{
    public class UpdateAuthorDTO
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Biography { get; set; } = string.Empty;
    }
}
