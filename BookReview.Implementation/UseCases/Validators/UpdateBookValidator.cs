using BookReview.Application.DTO;
using BookReview.DataAccess;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Validators
{
    public class UpdateBookValidator : AbstractValidator<UpdateBookDTO>
    {
        private const string required = "Field is required.";

        public UpdateBookValidator(
            BookReviewDbContext context)
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
                .WithMessage("ISBN cannot be longer than 20 characters.");

            RuleFor(x => x.PublishedYear)
                .NotEmpty()
                .WithMessage("Published year is required.")
                .InclusiveBetween(0, DateTime.UtcNow.Year)
                .WithMessage("Invalid publication year.");
        }
    }
}