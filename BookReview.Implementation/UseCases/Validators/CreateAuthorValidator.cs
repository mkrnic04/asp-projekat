using BookReview.Application.DTO;
using BookReview.DataAccess;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Validators
{
    public class CreateAuthorValidator : AbstractValidator<CreateAuthorDTO>
    {
        public CreateAuthorValidator(BookReviewDbContext context)
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

            RuleFor(x => x)
                .Must(x => !context.Authors.Any(a =>
                    a.FirstName == x.FirstName &&
                    a.LastName == x.LastName))
                .WithMessage("Author with this first name and last name already exists.");
        }
    }
}