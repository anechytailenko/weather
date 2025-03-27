using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNET.Backend.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixLocationTableColumnNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Country",
                table: "location",
                newName: "country");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "location",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "location",
                newName: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "country",
                table: "location",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "location",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "location",
                newName: "Id");
        }
    }
}
