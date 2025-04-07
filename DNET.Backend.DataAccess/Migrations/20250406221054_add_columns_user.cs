using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNET.Backend.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class add_columns_user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "password_reset_token",
                table: "user",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "password_reset_token_expires",
                table: "user",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "user",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password_reset_token",
                table: "user");

            migrationBuilder.DropColumn(
                name: "password_reset_token_expires",
                table: "user");

            migrationBuilder.DropColumn(
                name: "role",
                table: "user");
        }
    }
}
