using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;
using BookReview.DataAccess;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Validators
{
    public class CreateFavoriteValidator : AbstractValidator<CreateFavoriteDTO>
    {
        public CreateFavoriteValidator(BookReviewDbContext context)
        {
            this.RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User is required.")
                .Must(id => context.Users.Any(x => x.Id == id))
                .WithMessage("User does not exist.");

            RuleFor(x => x.BookId)
                .NotEmpty()
                .WithMessage("Book is required.")
                .Must(id => context.Books.Any(x => x.Id == id))
                .WithMessage("Book does not exist.");

            RuleFor(x => x)
                .Must(x => !context.Favorites.Any(f =>
                    f.UserId == x.UserId &&
                    f.BookId == x.BookId))
                .WithMessage("Book is already in favorites.");
        }
    }
}
