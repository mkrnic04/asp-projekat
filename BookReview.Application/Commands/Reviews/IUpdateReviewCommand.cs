using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;

namespace BookReview.Application.Commands.Reviews
{
    public interface IUpdateReviewCommand
    {
        string Name { get; }

        string Id { get; }

        void Execute(int id, UpdateReviewDTO data);
    }
}
