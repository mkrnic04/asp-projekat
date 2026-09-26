using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;
using BookReview.DataAccess;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Validators
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDTO>
    {
        public UpdateCategoryValidator(
            BookReviewDbContext context)
        {
            this.RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Category name is required.")
                .MaximumLength(50)
                .WithMessage("Category name cannot be longer than 50 characters.");
        }
    }
}
