using BookReview.Application.Commands.Authors;
using BookReview.Application.DTO;
using BookReview.DataAccess;
using BookReview.Domain;
using BookReview.Implementation.UseCases.Validators;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Commands.Authors
{
    public class EfCreateAuthorCommand : EfUseCase, ICreateAuthorCommand
    {
        private readonly CreateAuthorValidator _validator;

        public EfCreateAuthorCommand(
            CreateAuthorValidator validator,
            BookReviewDbContext context)
            : base(context)
        {
            _validator = validator;
        }

        public string Name => "Create new author";

        public string Id => "add-author";

        public void Execute(CreateAuthorDTO data)
        {
            _validator.ValidateAndThrow(data);

            Author author = new Author();

            author.FirstName = data.FirstName;
            author.LastName = data.LastName;
            author.Biography = data.Biography;

            ctx.Authors.Add(author);
            ctx.SaveChanges();
        }
    }
}