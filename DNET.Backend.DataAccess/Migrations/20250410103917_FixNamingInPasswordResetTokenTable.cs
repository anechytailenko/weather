using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNET.Backend.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixNamingInPasswordResetTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PasswordResetToken_user_UserId",
                table: "PasswordResetToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PasswordResetToken",
                table: "PasswordResetToken");

            migrationBuilder.RenameTable(
                name: "PasswordResetToken",
                newName: "password_reset_token");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "password_reset_token",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Expires",
                table: "password_reset_token",
                newName: "expires");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "password_reset_token",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "password_reset_token",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordResetToken_UserId",
                table: "password_reset_token",
                newName: "IX_password_reset_token_user_id");

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "password_reset_token",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_password_reset_token",
                table: "password_reset_token",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_password_reset_token_user_user_id",
                table: "password_reset_token",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_password_reset_token_user_user_id",
                table: "password_reset_token");

            migrationBuilder.DropPrimaryKey(
                name: "PK_password_reset_token",
                table: "password_reset_token");

            migrationBuilder.RenameTable(
                name: "password_reset_token",
                newName: "PasswordResetToken");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "PasswordResetToken",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "expires",
                table: "PasswordResetToken",
                newName: "Expires");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PasswordResetToken",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "PasswordResetToken",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_password_reset_token_user_id",
                table: "PasswordResetToken",
                newName: "IX_PasswordResetToken_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "PasswordResetToken",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PasswordResetToken",
                table: "PasswordResetToken",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordResetToken_user_UserId",
                table: "PasswordResetToken",
                column: "UserId",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
