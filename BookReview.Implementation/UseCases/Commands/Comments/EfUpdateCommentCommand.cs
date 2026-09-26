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
    public class EfUpdateCommentCommand : EfUseCase, IUpdateCommentCommand
    {
        private readonly UpdateCommentValidator _validator;

        public EfUpdateCommentCommand(
            UpdateCommentValidator validator,
            BookReviewDbContext context)
            : base(context)
        {
            _validator = validator;
        }

        public string Name => "Update comment";

        public string Id => "update-comment";

        public void Execute(int id, UpdateCommentDTO data)
        {
            _validator.ValidateAndThrow(data);

            var comment = ctx.Comments
                .FirstOrDefault(x => x.Id == id);

            if (comment == null)
            {
                throw new KeyNotFoundException("Comment not found.");
            }

            comment.Text = data.Text;

            ctx.SaveChanges();
        }
    }
}