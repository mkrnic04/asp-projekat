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
    public class EfUpdateReviewCommand : EfUseCase, IUpdateReviewCommand
    {
        private readonly UpdateReviewValidator _validator;

        public EfUpdateReviewCommand(
            UpdateReviewValidator validator,
            BookReviewDbContext context)
            : base(context)
        {
            _validator = validator;
        }

        public string Name => "Update review";

        public string Id => "update-review";

        public void Execute(int id, UpdateReviewDTO data)
        {
            _validator.ValidateAndThrow(data);

            var review = ctx.Reviews
                .FirstOrDefault(x => x.Id == id);

            if (review == null)
            {
                throw new KeyNotFoundException("Review not found.");
            }

            review.Title = data.Title;
            review.Text = data.Text;
            review.Rating = data.Rating;

            ctx.SaveChanges();
        }
    }
}
