using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.Commands.Reviews;
using BookReview.Application.DTO;
using BookReview.DataAccess;
using BookReview.Domain;
using BookReview.Implementation.UseCases.Validators;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Commands.Reviews
{
    public class EfCreateReviewCommand : EfUseCase, ICreateReviewCommand
    {
        private readonly CreateReviewValidator _validator;

        public EfCreateReviewCommand(
            CreateReviewValidator validator,
            BookReviewDbContext context)
            : base(context)
        {
            _validator = validator;
        }

        public string Name => "Create new review";

        public string Id => "add-review";

        public void Execute(CreateReviewDTO data)
        {
            _validator.ValidateAndThrow(data);

            Review review = new Review
            {
                BookId = data.BookId,
                UserId = data.UserId,
                Title = data.Title,
                Text = data.Text,
                Rating = data.Rating,
                CreatedAt = DateTime.UtcNow
            };

            ctx.Reviews.Add(review);
            ctx.SaveChanges();
        }
    }
}
