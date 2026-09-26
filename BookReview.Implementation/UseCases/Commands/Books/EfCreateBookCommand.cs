using BookReview.Application;
using BookReview.Application.Commands.Books;
using BookReview.Application.DTO;
using BookReview.DataAccess;
using BookReview.Domain;
using FluentValidation;
using BookReview.Implementation.UseCases.Validators;

namespace BookReview.Implementation.UseCases.Commands.Books
{
    public class EfCreateBookCommand : EfUseCase, ICreateBookCommand
    {
        private readonly CreateBookValidator _validator;

        public EfCreateBookCommand(
            CreateBookValidator validator,
            BookReviewDbContext context)
            : base(context)
        {
            _validator = validator;
        }

        public string Name => "Create new book";

        public string Id => "add-book";

        public int Execute(CreateBookDTO data)
        {
            _validator.ValidateAndThrow(data);

            Book book = new Book();

            book.Title = data.Title;
            book.Description = data.Description;
            book.ISBN = data.ISBN;
            book.PublishedYear = data.PublishedYear;

            book.BookAuthors.Add(new BookAuthor
            {
                AuthorId = data.AuthorId
            });

            book.BookCategories.Add(new BookCategory
            {
                CategoryId = data.CategoryId
            });

            ctx.Books.Add(book);
            ctx.SaveChanges();

            return book.Id;
        }
    }
}