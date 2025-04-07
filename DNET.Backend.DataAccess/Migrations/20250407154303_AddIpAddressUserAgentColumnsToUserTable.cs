using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNET.Backend.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddIpAddressUserAgentColumnsToUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ip_address",
                table: "user",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "user_agent",
                table: "user",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ip_address",
                table: "user");

            migrationBuilder.DropColumn(
                name: "user_agent",
                table: "user");
        }
    }
}
