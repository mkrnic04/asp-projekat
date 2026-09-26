using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;

namespace BookReview.Application.Commands.Users
{
    public interface IRegisterUserCommand
    {
        string Name { get; }
        string Id { get; }
        void Execute(RegisterUserDTO data);
    }
}
