using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.Commands.Books;
using BookReview.Application.DTO;
using BookReview.DataAccess;
using BookReview.Implementation.UseCases.Validators;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Commands.Books
{
    public class EfUploadBookCoverCommand
        : EfUseCase, IUploadBookCoverCommand
    {
        private readonly UploadBookCoverValidator _validator;

        public EfUploadBookCoverCommand(
            UploadBookCoverValidator validator,
            BookReviewDbContext context)
            : base(context)
        {
            _validator = validator;
        }

        public string Name => "Upload book cover";

        public string Id => "upload-book-cover";

        public void Execute(
            int bookId,
            UploadBookCoverDTO data)
        {
            _validator.ValidateAndThrow(data);

            var book = ctx.Books
                .FirstOrDefault(x => x.Id == bookId);

            if (book == null)
            {
                throw new KeyNotFoundException(
                    "Book not found.");
            }

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "books");

            Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(data.FileName)
                .ToLowerInvariant();

            var fileName =
                Guid.NewGuid().ToString() + extension;

            var filePath = Path.Combine(
                uploadsFolder,
                fileName);

            File.WriteAllBytes(filePath, data.File);

            book.CoverImagePath =
                "/uploads/books/" + fileName;

            ctx.SaveChanges();
        }
    }
}