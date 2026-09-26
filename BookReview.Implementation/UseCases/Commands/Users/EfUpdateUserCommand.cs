using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.Commands.Users;
using BookReview.Application.DTO;
using BookReview.DataAccess;
using BookReview.Domain;
using BookReview.Implementation.UseCases.Validators;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Commands.Users
{
    public class EfUpdateUserCommand : EfUseCase, IUpdateUserCommand
    {
        private readonly UpdateUserValidator _validator;

        public EfUpdateUserCommand(
            UpdateUserValidator validator,
            BookReviewDbContext context)
            : base(context)
        {
            _validator = validator;
        }

        public string Name => "Update user";

        public string Id => "update-user";

        public void Execute(int id, UpdateUserDTO data)
        {
            _validator.ValidateAndThrow(data);

            var user = ctx.Users
                .FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var usernameExists = ctx.Users.Any(x =>
                x.Username == data.Username &&
                x.Id != id);

            if (usernameExists)
            {
                throw new ValidationException(
                    "Username is already in use.");
            }

            var emailExists = ctx.Users.Any(x =>
                x.Email == data.Email &&
                x.Id != id);

            if (emailExists)
            {
                throw new ValidationException(
                    "Email is already in use.");
            }

            user.Username = data.Username;
            user.Email = data.Email;
            user.FirstName = data.FirstName;
            user.LastName = data.LastName;
            user.Role = data.Role;

            ctx.SaveChanges();
        }
    }
}
