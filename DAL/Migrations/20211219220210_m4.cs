using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class m4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GamePhase",
                table: "GameSaves",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GamePhase",
                table: "GameSaves");
        }
    }
}
