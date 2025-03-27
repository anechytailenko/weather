using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNET.Backend.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_weather_location_id",
                table: "weather",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_alert_location_id",
                table: "alert",
                column: "location_id");

            migrationBuilder.AddForeignKey(
                name: "FK_alert_location_location_id",
                table: "alert",
                column: "location_id",
                principalTable: "location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_weather_location_location_id",
                table: "weather",
                column: "location_id",
                principalTable: "location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_alert_location_location_id",
                table: "alert");

            migrationBuilder.DropForeignKey(
                name: "FK_weather_location_location_id",
                table: "weather");

            migrationBuilder.DropIndex(
                name: "IX_weather_location_id",
                table: "weather");

            migrationBuilder.DropIndex(
                name: "IX_alert_location_id",
                table: "alert");
        }
    }
}
