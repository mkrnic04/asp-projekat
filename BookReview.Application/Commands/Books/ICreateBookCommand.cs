using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;

namespace BookReview.Application.Commands.Books
{
    public interface ICreateBookCommand
    {
        string Name { get; }

        string Id { get; }

        int Execute(CreateBookDTO data);
    }
}