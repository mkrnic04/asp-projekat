using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;

namespace BookReview.Application.Commands.Comments
{
    public interface ICreateCommentCommand
    {
        string Name { get; }

        string Id { get; }

        void Execute(CreateCommentDTO data);
    }
}
