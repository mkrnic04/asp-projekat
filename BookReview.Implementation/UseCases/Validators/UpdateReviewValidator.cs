using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;
using BookReview.DataAccess;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Validators
{
    public class UpdateReviewValidator : AbstractValidator<UpdateReviewDTO>
    {
        public UpdateReviewValidator(
            BookReviewDbContext context)
        {
            this.RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Review title is required.")
                .MaximumLength(150)
                .WithMessage("Review title cannot be longer than 150 characters.");

            RuleFor(x => x.Text)
                .NotEmpty()
                .WithMessage("Review text is required.")
                .MaximumLength(3000)
                .WithMessage("Review text cannot be longer than 3000 characters.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5.");
        }
    }
}
