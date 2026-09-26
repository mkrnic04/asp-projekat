using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.Commands.Authors;
using BookReview.Application.DTO;
using BookReview.DataAccess;
using BookReview.Domain;
using BookReview.Implementation.UseCases.Validators;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Commands.Authors
{
    public class EfUpdateAuthorCommand : EfUseCase, IUpdateAuthorCommand
    {
        private readonly UpdateAuthorValidator _validator;

        public EfUpdateAuthorCommand(
            UpdateAuthorValidator validator,
            BookReviewDbContext context)
            : base(context)
        {
            _validator = validator;
        }

        public string Name => "Update author";

        public string Id => "update-author";

        public void Execute(int id, UpdateAuthorDTO data)
        {
            _validator.ValidateAndThrow(data);

            var author = ctx.Authors
                .FirstOrDefault(x => x.Id == id);

            if (author == null)
            {
                throw new KeyNotFoundException("Author not found.");
            }

            var authorExists = ctx.Authors.Any(x =>
                x.FirstName == data.FirstName &&
                x.LastName == data.LastName &&
                x.Id != id);

            if (authorExists)
            {
                throw new ValidationException(
                    "Author with this first name and last name already exists.");
            }

            author.FirstName = data.FirstName;
            author.LastName = data.LastName;
            author.Biography = data.Biography;

            ctx.SaveChanges();
        }
    }
}
