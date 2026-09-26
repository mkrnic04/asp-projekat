using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookReview.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthTokenRevocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAt",
                table: "AuthTokens",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevokedAt",
                table: "AuthTokens");
        }
    }
}
