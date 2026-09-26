using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;
using BookReview.DataAccess;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Validators
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserDTO>
    {
        private const string required = "Field is required.";

        public RegisterUserValidator(BookReviewDbContext context)
        {
            this.RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(required)
                .EmailAddress()
                .WithMessage("Invalid email address.")
                .MaximumLength(100)
                .WithMessage("Email cannot be longer than 100 characters.")
                .Must(x => !context.Users.Any(u => u.Email == x))
                .WithMessage("Email is already in use.");

            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage(required)
                .MinimumLength(3)
                .WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(50)
                .WithMessage("Username cannot be longer than 50 characters.")
                .Matches("^[A-Za-z0-9_]+$")
                .WithMessage("Username can only contain letters, numbers, and underscores.")
                .Must(x => !context.Users.Any(u => u.Username == x))
                .WithMessage("Username is already in use.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(required)
                .MinimumLength(2)
                .WithMessage("First name must be at least 2 characters long.")
                .MaximumLength(50)
                .WithMessage("First name cannot be longer than 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(required)
                .MinimumLength(2)
                .WithMessage("Last name must be at least 2 characters long.")
                .MaximumLength(50)
                .WithMessage("Last name cannot be longer than 50 characters.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(required)
                .Matches(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{8,}$")
                .WithMessage("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
        }
    }
}
