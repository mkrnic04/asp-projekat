using BookReview.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.DataAccess.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(3000);

            builder.Property(x => x.ISBN)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(x => x.ISBN)
                .IsUnique();

            builder.Property(x => x.PublishedYear)
                .IsRequired();

            builder.Property(x => x.CoverImagePath)
                .HasMaxLength(500)
                .IsRequired(false);
        }
    }
}
