using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class m2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameShipConfigId",
                table: "GameShip",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameShip_GameShipConfigId",
                table: "GameShip",
                column: "GameShipConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameShip_GameShipConfig_GameShipConfigId",
                table: "GameShip",
                column: "GameShipConfigId",
                principalTable: "GameShipConfig",
                principalColumn: "GameShipConfigId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameShip_GameShipConfig_GameShipConfigId",
                table: "GameShip");

            migrationBuilder.DropIndex(
                name: "IX_GameShip_GameShipConfigId",
                table: "GameShip");

            migrationBuilder.DropColumn(
                name: "GameShipConfigId",
                table: "GameShip");
        }
    }
}
