using BookReview.Application;
using System.Collections.Generic;

namespace BookReview.API.JWT
{
    public class JwtUser : IApplicationUser
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public IEnumerable<string> AllowedUseCases { get; set; }
            = new List<string>();
    }
}