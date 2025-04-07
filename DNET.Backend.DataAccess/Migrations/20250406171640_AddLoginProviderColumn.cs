using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNET.Backend.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginProviderColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "login_provider",
                table: "user",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "login_provider",
                table: "user");
        }
    }
}
