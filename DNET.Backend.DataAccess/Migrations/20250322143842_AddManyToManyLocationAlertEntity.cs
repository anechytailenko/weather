using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNET.Backend.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddManyToManyLocationAlertEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "location_alert",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "integer", nullable: false),
                    AlertId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_alert", x => new { x.LocationId, x.AlertId });
                    table.ForeignKey(
                        name: "FK_location_alert_alert_AlertId",
                        column: x => x.AlertId,
                        principalTable: "alert",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_location_alert_location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "location",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_location_alert_AlertId",
                table: "location_alert",
                column: "AlertId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "location_alert");
        }
    }
}
