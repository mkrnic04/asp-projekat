using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Domain
{
    public class UserProfile : BaseEntity
    {
        public int UserId { get; set; }

        public string Biography { get; set; } = string.Empty;

        public virtual User User { get; set; } = null!;
    }
}
