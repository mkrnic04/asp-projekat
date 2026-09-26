using BookReview.DataAccess;
using BookReview.Domain;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace BookReview.API.Seed
{
    public static class DatabaseSeeder
    {
        public static void Seed(BookReviewDbContext context)
        {
            // Ako već postoje korisnici
            if (context.Users.Any())
            {
                return;
            }

            var admin = new User
            {
                Username = "admin",
                Email = "admin@gmail.com",
                FirstName = "Admin",
                LastName = "User",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Admin",
                RegisteredAt = DateTime.UtcNow
            };

            var user = new User
            {
                Username = "milica",
                Email = "milica@gmail.com",
                FirstName = "Milica",
                LastName = "Petrovic",
                Password = BCrypt.Net.BCrypt.HashPassword("User123!"),
                Role = "User",
                RegisteredAt = DateTime.UtcNow
            };

            context.Users.AddRange(admin, user);
            context.SaveChanges();

            var adminProfile = new UserProfile
            {
                UserId = admin.Id,
                Biography = "Administrator aplikacije."
            };

            var userProfile = new UserProfile
            {
                UserId = user.Id,
                Biography = "Ljubitelj knjiga."
            };

            context.UserProfiles.AddRange(adminProfile, userProfile);

            var fantasy = new Category
            {
                Name = "Fantasy"
            };

            var drama = new Category
            {
                Name = "Drama"
            };

            var mystery = new Category
            {
                Name = "Mystery"
            };

            context.Categories.AddRange(
                fantasy,
                drama,
                mystery
            );

            var rowling = new Author
            {
                FirstName = "J. K.",
                LastName = "Rowling",
                Biography = "British author."
            };

            var orwell = new Author
            {
                FirstName = "George",
                LastName = "Orwell",
                Biography = "English novelist and essayist."
            };

            var christie = new Author
            {
                FirstName = "Agatha",
                LastName = "Christie",
                Biography = "English mystery novelist."
            };

            context.Authors.AddRange(
                rowling,
                orwell,
                christie
            );

            var book1 = new Book
            {
                Title = "Harry Potter and the Philosopher's Stone",
                Description = "The first book in the Harry Potter series.",
                ISBN = "9780747532699",
                PublishedYear = 1997
            };

            var book2 = new Book
            {
                Title = "1984",
                Description = "A dystopian novel about a totalitarian society.",
                ISBN = "9780451524935",
                PublishedYear = 1949
            };

            var book3 = new Book
            {
                Title = "Murder on the Orient Express",
                Description = "A famous Hercule Poirot mystery.",
                ISBN = "9780062693662",
                PublishedYear = 1934
            };

            context.Books.AddRange(book1, book2, book3);
            context.SaveChanges();

            context.BookAuthors.AddRange(
                new BookAuthor
                {
                    BookId = book1.Id,
                    AuthorId = rowling.Id
                },
                new BookAuthor
                {
                    BookId = book2.Id,
                    AuthorId = orwell.Id
                },
                new BookAuthor
                {
                    BookId = book3.Id,
                    AuthorId = christie.Id
                }
            );

            context.BookCategories.AddRange(
                new BookCategory
                {
                    BookId = book1.Id,
                    CategoryId = fantasy.Id
                },
                new BookCategory
                {
                    BookId = book2.Id,
                    CategoryId = drama.Id
                },
                new BookCategory
                {
                    BookId = book3.Id,
                    CategoryId = mystery.Id
                }
            );

            context.Reviews.Add(
                new Review
                {
                    BookId = book1.Id,
                    UserId = user.Id,
                    Title = "Odlična knjiga",
                    Text = "Veoma zanimljiva knjiga.",
                    Rating = 5,
                    CreatedAt = DateTime.UtcNow
                }
            );

            context.SaveChanges();
        }
    }
}