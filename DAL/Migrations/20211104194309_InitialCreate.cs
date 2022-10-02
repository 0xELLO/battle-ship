using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameConfigs",
                columns: table => new
                {
                    GameConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    BoardSizeX = table.Column<int>(type: "int", nullable: false),
                    BoardSizeY = table.Column<int>(type: "int", nullable: false),
                    EShipTouchRule = table.Column<int>(type: "int", nullable: false),
                    ShipConfigurations = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameConfigs", x => x.GameConfigId);
                });

            migrationBuilder.CreateTable(
                name: "GameSaves",
                columns: table => new
                {
                    GameSaveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaveName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    GameCurrentPlayerNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameMovesNumber = table.Column<int>(type: "int", nullable: false),
                    FirstGameBoard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondGameBoard = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSaves", x => x.GameSaveId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameConfigs");

            migrationBuilder.DropTable(
                name: "GameSaves");
        }
    }
}
