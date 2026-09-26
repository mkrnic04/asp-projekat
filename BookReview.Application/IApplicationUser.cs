using System;
using System.Collections.Generic;
using System.Text;

using System.Collections.Generic;

namespace BookReview.Application
{
    public interface IApplicationUser
    {
        int Id { get; }
        string Username { get; }
        string Email { get; }

        IEnumerable<string> AllowedUseCases { get; }
    }
}
