using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;

namespace BookReview.Application.Commands.Comments
{
    public interface IUpdateCommentCommand
    {
        string Name { get; }

        string Id { get; }

        void Execute(int id, UpdateCommentDTO data);
    }
}
