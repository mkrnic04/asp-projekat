using BookReview.Application.DTO;

namespace BookReview.Application.Commands.Books
{
    public interface IUpdateBookCommand
    {
        string Name { get; }

        string Id { get; }

        void Execute(int id, UpdateBookDTO data);
    }
}