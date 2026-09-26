using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Application.DTO
{
    public class UpdateUserDTO
    {
        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
