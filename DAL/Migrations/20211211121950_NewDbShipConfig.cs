using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class NewDbShipConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShipConfigurations",
                table: "GameConfigs");

            migrationBuilder.AddColumn<int>(
                name: "GameConfigId",
                table: "GameSaves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GameShipConfig",
                columns: table => new
                {
                    GameShipConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ShipSizeX = table.Column<int>(type: "int", nullable: false),
                    ShipSizeY = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameShipConfig", x => x.GameShipConfigId);
                });

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
                name: "IX_GameSaves_GameConfigId",
                table: "GameSaves",
                column: "GameConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipConfigInGameConfig_GameConfigId",
                table: "ShipConfigInGameConfig",
                column: "GameConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipConfigInGameConfig_GameShipConfigId",
                table: "ShipConfigInGameConfig",
                column: "GameShipConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameSaves_GameConfigs_GameConfigId",
                table: "GameSaves",
                column: "GameConfigId",
                principalTable: "GameConfigs",
                principalColumn: "GameConfigId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameSaves_GameConfigs_GameConfigId",
                table: "GameSaves");

            migrationBuilder.DropTable(
                name: "ShipConfigInGameConfig");

            migrationBuilder.DropTable(
                name: "GameShipConfig");

            migrationBuilder.DropIndex(
                name: "IX_GameSaves_GameConfigId",
                table: "GameSaves");

            migrationBuilder.DropColumn(
                name: "GameConfigId",
                table: "GameSaves");

            migrationBuilder.AddColumn<string>(
                name: "ShipConfigurations",
                table: "GameConfigs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
