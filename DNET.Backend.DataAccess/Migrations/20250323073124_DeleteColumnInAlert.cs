using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNET.Backend.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DeleteColumnInAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_alert_location_LocationEntityId",
                table: "alert");

            migrationBuilder.DropIndex(
                name: "IX_alert_LocationEntityId",
                table: "alert");

            migrationBuilder.DropColumn(
                name: "LocationEntityId",
                table: "alert");

            migrationBuilder.DropColumn(
                name: "location_id",
                table: "alert");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "location_alert",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "location_alert");

            migrationBuilder.AddColumn<int>(
                name: "LocationEntityId",
                table: "alert",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "location_id",
                table: "alert",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_alert_LocationEntityId",
                table: "alert",
                column: "LocationEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_alert_location_LocationEntityId",
                table: "alert",
                column: "LocationEntityId",
                principalTable: "location",
                principalColumn: "id");
        }
    }
}
