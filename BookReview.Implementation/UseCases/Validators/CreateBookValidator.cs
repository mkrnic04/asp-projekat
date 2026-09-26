using BookReview.Application.DTO;
using BookReview.DataAccess;
using FluentValidation;
using System;

namespace BookReview.Implementation.UseCases.Validators
{
    public class CreateBookValidator : AbstractValidator<CreateBookDTO>
    {
        public CreateBookValidator(BookReviewDbContext context)
        {
            this.RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(200)
                .WithMessage("Title cannot be longer than 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(3000)
                .WithMessage("Description cannot be longer than 3000 characters.");

            RuleFor(x => x.ISBN)
                .NotEmpty()
                .WithMessage("ISBN is required.")
                .MaximumLength(20)
                .WithMessage("ISBN cannot be longer than 20 characters.")
                .Must(x => !context.Books.Any(b => b.ISBN == x))
                .WithMessage("ISBN is already in use.");

            RuleFor(x => x.PublishedYear)
                .NotEmpty()
                .WithMessage("Published year is required.")
                .InclusiveBetween(0, DateTime.UtcNow.Year)
                .WithMessage("Invalid publication year.");

            RuleFor(x => x.AuthorId)
                .GreaterThan(0)
                .WithMessage("Author is required.")
                .Must(id => context.Authors.Any(a => a.Id == id))
                .WithMessage("Selected author does not exist.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage("Category is required.")
                .Must(id => context.Categories.Any(c => c.Id == id))
                .WithMessage("Selected category does not exist.");
        }
    }
}