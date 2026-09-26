using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;
using BookReview.DataAccess;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Validators
{
    public class CreateCommentValidator : AbstractValidator<CreateCommentDTO>
    {
        public CreateCommentValidator(BookReviewDbContext context)
        {
            this.RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.ReviewId)
                .NotEmpty()
                .WithMessage("Review is required.")
                .Must(id => context.Reviews.Any(x => x.Id == id))
                .WithMessage("Review does not exist.");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User is required.")
                .Must(id => context.Users.Any(x => x.Id == id))
                .WithMessage("User does not exist.");

            RuleFor(x => x.Text)
                .NotEmpty()
                .WithMessage("Text is required.")
                .MaximumLength(1000)
                .WithMessage("Text can have a maximum of 1000 characters.");
        }
    }
}
