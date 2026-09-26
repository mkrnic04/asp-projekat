using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;

namespace BookReview.Application.Commands.Reviews
{
    public interface ICreateReviewCommand
    {
        string Name { get; }

        string Id { get; }

        void Execute(CreateReviewDTO data);
    }
}
