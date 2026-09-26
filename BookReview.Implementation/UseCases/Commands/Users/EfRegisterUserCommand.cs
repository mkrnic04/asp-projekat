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
    public class EfRegisterUserCommand : EfUseCase, IRegisterUserCommand
    {
        private readonly RegisterUserValidator _validator;

        public EfRegisterUserCommand(
            RegisterUserValidator validator,
            BookReviewDbContext context)
            : base(context)
        {
            _validator = validator;
        }

        public string Name => "Register user";
        public string Id => "register-user";

        public void Execute(RegisterUserDTO data)
        {
            _validator.ValidateAndThrow(data);

            User user = new User
            {
                Username = data.Username,
                Email = data.Email,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(data.Password),
                Role = "User",
                RegisteredAt = DateTime.UtcNow
            };

            //Console.WriteLine($"REGISTERED AT: {user.RegisteredAt}");

            ctx.Users.Add(user);
            ctx.SaveChanges();
        }
    }
}
