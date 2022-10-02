using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class SHIPs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameShip",
                columns: table => new
                {
                    GameShipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Coordinates = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPlaced = table.Column<int>(type: "int", nullable: false),
                    GameSaveId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameShip", x => x.GameShipId);
                    table.ForeignKey(
                        name: "FK_GameShip_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "GameSaveId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameShip_GameSaveId",
                table: "GameShip",
                column: "GameSaveId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameShip");
        }
    }
}
