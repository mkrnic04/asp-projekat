using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace BookReview.Domain
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        public DateTime? RegisteredAt { get; set; }

        public virtual UserProfile? UserProfile { get; set; }

        public virtual HashSet<Review> Reviews { get; set; } = new();

        public virtual HashSet<Comment> Comments { get; set; } = new();

        public virtual HashSet<Favorite> Favorites { get; set; } = new();

        public virtual HashSet<AuthToken> AuthTokens { get; set; } = new();
    }
}
