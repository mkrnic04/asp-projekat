using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.Commands.Favorites;
using BookReview.Application.DTO;
using BookReview.DataAccess;
using BookReview.Domain;
using BookReview.Implementation.UseCases.Validators;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Commands.Favorites
{
    public class EfCreateFavoriteCommand : EfUseCase, ICreateFavoriteCommand
    {
        private readonly CreateFavoriteValidator _validator;

        public EfCreateFavoriteCommand(
            CreateFavoriteValidator validator,
            BookReviewDbContext context)
            : base(context)
        {
            _validator = validator;
        }

        public string Name => "Create new favorite";

        public string Id => "add-favorite";

        public void Execute(CreateFavoriteDTO data)
        {
            _validator.ValidateAndThrow(data);

            Favorite favorite = new Favorite
            {
                UserId = data.UserId,
                BookId = data.BookId,
                CreatedAt = DateTime.UtcNow
            };

            ctx.Favorites.Add(favorite);
            ctx.SaveChanges();
        }
    }
}
