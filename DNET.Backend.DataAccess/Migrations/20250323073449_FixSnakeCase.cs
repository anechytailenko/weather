using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNET.Backend.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_location_alert_alert_AlertId",
                table: "location_alert");

            migrationBuilder.DropForeignKey(
                name: "FK_location_alert_location_LocationId",
                table: "location_alert");

            migrationBuilder.RenameColumn(
                name: "AlertId",
                table: "location_alert",
                newName: "alert_id");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "location_alert",
                newName: "location_id");

            migrationBuilder.RenameIndex(
                name: "IX_location_alert_AlertId",
                table: "location_alert",
                newName: "IX_location_alert_alert_id");

            migrationBuilder.AddForeignKey(
                name: "FK_location_alert_alert_alert_id",
                table: "location_alert",
                column: "alert_id",
                principalTable: "alert",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_location_alert_location_location_id",
                table: "location_alert",
                column: "location_id",
                principalTable: "location",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_location_alert_alert_alert_id",
                table: "location_alert");

            migrationBuilder.DropForeignKey(
                name: "FK_location_alert_location_location_id",
                table: "location_alert");

            migrationBuilder.RenameColumn(
                name: "alert_id",
                table: "location_alert",
                newName: "AlertId");

            migrationBuilder.RenameColumn(
                name: "location_id",
                table: "location_alert",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_location_alert_alert_id",
                table: "location_alert",
                newName: "IX_location_alert_AlertId");

            migrationBuilder.AddForeignKey(
                name: "FK_location_alert_alert_AlertId",
                table: "location_alert",
                column: "AlertId",
                principalTable: "alert",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_location_alert_location_LocationId",
                table: "location_alert",
                column: "LocationId",
                principalTable: "location",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
