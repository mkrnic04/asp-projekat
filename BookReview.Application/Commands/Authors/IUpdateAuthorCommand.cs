using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;

namespace BookReview.Application.Commands.Authors
{
    public interface IUpdateAuthorCommand
    {
        string Name { get; }

        string Id { get; }

        void Execute(int id, UpdateAuthorDTO data);
    }
}
