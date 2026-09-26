using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;
using BookReview.DataAccess;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Validators
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDTO>
    {
        public CreateCategoryValidator(BookReviewDbContext context)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Category name is required.")
                .MaximumLength(50)
                .WithMessage("Category name cannot be longer than 50 characters.")
                .Must(x => !context.Categories.Any(c => c.Name == x))
                .WithMessage("Category with this name already exists.");
        }
    }
}
