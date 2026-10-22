using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroShop.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTermsAcceptedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "terms_accepted_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "terms_accepted_at",
                table: "users");
        }
    }
}
