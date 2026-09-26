using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;
using BookReview.DataAccess;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Validators
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewDTO>
    {
        public CreateReviewValidator(BookReviewDbContext context)
        {
            this.RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.BookId)
                .NotEmpty()
                .WithMessage("Book is required.")
                .Must(id => context.Books.Any(x => x.Id == id))
                .WithMessage("Book does not exist.");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User is required.")
                .Must(id => context.Users.Any(x => x.Id == id))
                .WithMessage("User does not exist.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(150)
                .WithMessage("Title can have a maximum of 150 characters.");

            RuleFor(x => x.Text)
                .NotEmpty()
                .WithMessage("Text is required.")
                .MaximumLength(3000)
                .WithMessage("Text can have a maximum of 3000 characters.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x)
                .Must(x => !context.Reviews.Any(r =>
                    r.BookId == x.BookId &&
                    r.UserId == x.UserId))
                .WithMessage("User already reviewed this book.");
        }
    }
}
