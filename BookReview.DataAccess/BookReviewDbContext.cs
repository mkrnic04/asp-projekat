using BookReview.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookReview.DataAccess
{
    public class BookReviewDbContext : DbContext
    {
        private string _connString;

        public BookReviewDbContext(string connString)
        {
            _connString = connString;
        }

        public BookReviewDbContext()
        {
            _connString =
                @"Server=(localdb)\MSSQLLocalDB;Database=BookReview;Trusted_Connection=True;TrustServerCertificate=True";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(_connString)
                .UseLazyLoadingProxies();

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(BookReviewDbContext).Assembly
            );

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Favorite> Favorites { get; set; }

        public DbSet<BookAuthor> BookAuthors { get; set; }

        public DbSet<BookCategory> BookCategories { get; set; }

        public DbSet<AuthToken> AuthTokens { get; set; }
    }
}
