using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNET.Backend.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_alert_Location_location_id",
                table: "alert");

            migrationBuilder.DropForeignKey(
                name: "FK_location_alert_Location_location_id",
                table: "location_alert");

            migrationBuilder.DropForeignKey(
                name: "FK_location_alert_alert_alert_id",
                table: "location_alert");

            migrationBuilder.DropForeignKey(
                name: "FK_weather_Location_location_id",
                table: "weather");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Location",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_alert_location_id",
                table: "alert");

            migrationBuilder.RenameTable(
                name: "Location",
                newName: "location");

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

            migrationBuilder.AlterColumn<string>(
                name: "country",
                table: "location",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "city",
                table: "location",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "LocationEntityId",
                table: "alert",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_location",
                table: "location",
                column: "id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_weather_location_location_id",
                table: "weather",
                column: "location_id",
                principalTable: "location",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_alert_location_LocationEntityId",
                table: "alert");

            migrationBuilder.DropForeignKey(
                name: "FK_location_alert_alert_AlertId",
                table: "location_alert");

            migrationBuilder.DropForeignKey(
                name: "FK_location_alert_location_LocationId",
                table: "location_alert");

            migrationBuilder.DropForeignKey(
                name: "FK_weather_location_location_id",
                table: "weather");

            migrationBuilder.DropPrimaryKey(
                name: "PK_location",
                table: "location");

            migrationBuilder.DropIndex(
                name: "IX_alert_LocationEntityId",
                table: "alert");

            migrationBuilder.DropColumn(
                name: "LocationEntityId",
                table: "alert");

            migrationBuilder.RenameTable(
                name: "location",
                newName: "Location");

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

            migrationBuilder.RenameColumn(
                name: "country",
                table: "Location",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "Location",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Location",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Location",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Location",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Location",
                table: "Location",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_alert_location_id",
                table: "alert",
                column: "location_id");

            migrationBuilder.AddForeignKey(
                name: "FK_alert_Location_location_id",
                table: "alert",
                column: "location_id",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_location_alert_Location_location_id",
                table: "location_alert",
                column: "location_id",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_location_alert_alert_alert_id",
                table: "location_alert",
                column: "alert_id",
                principalTable: "alert",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_weather_Location_location_id",
                table: "weather",
                column: "location_id",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
