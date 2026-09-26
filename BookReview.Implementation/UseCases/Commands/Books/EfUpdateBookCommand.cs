using BookReview.Application.Commands.Books;
using BookReview.Application.DTO;
using BookReview.DataAccess;
using BookReview.Domain;
using BookReview.Implementation.UseCases.Validators;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Commands.Books
{
    public class EfUpdateBookCommand : EfUseCase, IUpdateBookCommand
    {
        private readonly UpdateBookValidator _validator;

        public EfUpdateBookCommand(
            UpdateBookValidator validator,
            BookReviewDbContext context)
            : base(context)
        {
            _validator = validator;
        }

        public string Name => "Update book";

        public string Id => "update-book";

        public void Execute(int id, UpdateBookDTO data)
        {
            _validator.ValidateAndThrow(data);

            var book = ctx.Books
                .FirstOrDefault(x => x.Id == id);

            if (book == null)
            {
                throw new KeyNotFoundException("Book not found.");
            }

            var isbnExists = ctx.Books.Any(x =>
                x.ISBN == data.ISBN &&
                x.Id != id);

            if (isbnExists)
            {
                throw new ValidationException(
                    "ISBN is already in use.");
            }

            book.Title = data.Title;
            book.Description = data.Description;
            book.ISBN = data.ISBN;
            book.PublishedYear = data.PublishedYear;

            ctx.SaveChanges();
        }
    }
}