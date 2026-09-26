using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;
using BookReview.DataAccess;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Validators
{
    public class UpdateAuthorValidator : AbstractValidator<UpdateAuthorDTO>
    {
        public UpdateAuthorValidator(
            BookReviewDbContext context)
        {
            this.RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .MaximumLength(50)
                .WithMessage("First name can have a maximum of 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .MaximumLength(50)
                .WithMessage("Last name can have a maximum of 50 characters.");

            RuleFor(x => x.Biography)
                .MaximumLength(2000)
                .WithMessage("Biography can have a maximum of 2000 characters.");
        }
    }
}
