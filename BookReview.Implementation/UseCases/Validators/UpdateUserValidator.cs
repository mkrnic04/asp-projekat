using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;
using BookReview.DataAccess;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserDTO>
    {
        public UpdateUserValidator(
            BookReviewDbContext context)
        {
            this.RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required.")
                .MinimumLength(3)
                .WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(50)
                .WithMessage("Username cannot be longer than 50 characters.")
                .Matches("^[A-Za-z0-9_]+$")
                .WithMessage("Username can only contain letters, numbers, and underscores.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email address.")
                .MaximumLength(100)
                .WithMessage("Email cannot be longer than 100 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .MinimumLength(2)
                .WithMessage("First name must be at least 2 characters long.")
                .MaximumLength(50)
                .WithMessage("First name cannot be longer than 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .MinimumLength(2)
                .WithMessage("Last name must be at least 2 characters long.")
                .MaximumLength(50)
                .WithMessage("Last name cannot be longer than 50 characters.");

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role is required.")
                .MaximumLength(20)
                .WithMessage("Role cannot be longer than 20 characters.")
                .Must(x => x == "User" || x == "Admin")
                .WithMessage("Role must be User or Admin.");
        }
    }
}
