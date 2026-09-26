using System;
using System.Collections.Generic;
using System.Text;


namespace BookReview.Domain
{
    public class AuthToken : BaseEntity
    {
        public string TokenId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public int UserId { get; set; }

        public int? JwtTokenId { get; set; }

        public virtual User User { get; set; } = null!;

        public virtual AuthToken? JwtToken { get; set; }
    }
}
