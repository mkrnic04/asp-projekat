using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.Commands.Comments;
using BookReview.Application.DTO;
using BookReview.DataAccess;
using BookReview.Domain;
using BookReview.Implementation.UseCases.Validators;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Commands.Comments
{
    public class EfCreateCommentCommand : EfUseCase, ICreateCommentCommand
    {
        private readonly CreateCommentValidator _validator;

        public EfCreateCommentCommand(
            CreateCommentValidator validator,
            BookReviewDbContext context)
            : base(context)
        {
            _validator = validator;
        }

        public string Name => "Create new comment";

        public string Id => "add-comment";

        public void Execute(CreateCommentDTO data)
        {
            _validator.ValidateAndThrow(data);

            Comment comment = new Comment
            {
                ReviewId = data.ReviewId,
                UserId = data.UserId,
                Text = data.Text,
                CreatedAt = DateTime.UtcNow
            };

            ctx.Comments.Add(comment);
            ctx.SaveChanges();
        }
    }
}
