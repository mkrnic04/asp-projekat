using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookReview.DataAccess.Configurations
{
    public class AuthTokenConfiguration : IEntityTypeConfiguration<AuthToken>
    {
        public void Configure(EntityTypeBuilder<AuthToken> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TokenId)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.RevokedAt)
                .IsRequired(false);

            builder.HasIndex(x => x.TokenId)
                .IsUnique();

            builder.HasOne(x => x.User)
                .WithMany(x => x.AuthTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.JwtToken)
                .WithOne()
                .HasForeignKey<AuthToken>(x => x.JwtTokenId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
