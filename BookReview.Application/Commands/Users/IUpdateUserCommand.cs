using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;

namespace BookReview.Application.Commands.Users
{
    public interface IUpdateUserCommand
    {
        string Name { get; }

        string Id { get; }

        void Execute(int id, UpdateUserDTO data);
    }
}
