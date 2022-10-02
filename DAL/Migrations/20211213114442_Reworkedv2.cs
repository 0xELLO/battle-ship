using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class Reworkedv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipConfigInGameConfig");

            migrationBuilder.AddColumn<int>(
                name: "GameConfigId",
                table: "GameShipConfig",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameShipConfig_GameConfigId",
                table: "GameShipConfig",
                column: "GameConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameShipConfig_GameConfigs_GameConfigId",
                table: "GameShipConfig",
                column: "GameConfigId",
                principalTable: "GameConfigs",
                principalColumn: "GameConfigId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameShipConfig_GameConfigs_GameConfigId",
                table: "GameShipConfig");

            migrationBuilder.DropIndex(
                name: "IX_GameShipConfig_GameConfigId",
                table: "GameShipConfig");

            migrationBuilder.DropColumn(
                name: "GameConfigId",
                table: "GameShipConfig");

            migrationBuilder.CreateTable(
                name: "ShipConfigInGameConfig",
                columns: table => new
                {
                    ShipConfigInGameConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameConfigId = table.Column<int>(type: "int", nullable: false),
                    GameShipConfigId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipConfigInGameConfig", x => x.ShipConfigInGameConfigId);
                    table.ForeignKey(
                        name: "FK_ShipConfigInGameConfig_GameConfigs_GameConfigId",
                        column: x => x.GameConfigId,
                        principalTable: "GameConfigs",
                        principalColumn: "GameConfigId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipConfigInGameConfig_GameShipConfig_GameShipConfigId",
                        column: x => x.GameShipConfigId,
                        principalTable: "GameShipConfig",
                        principalColumn: "GameShipConfigId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipConfigInGameConfig_GameConfigId",
                table: "ShipConfigInGameConfig",
                column: "GameConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipConfigInGameConfig_GameShipConfigId",
                table: "ShipConfigInGameConfig",
                column: "GameShipConfigId");
        }
    }
}
