using Microsoft.EntityFrameworkCore.Migrations;

namespace Botflox.Bot.Migrations
{
    public partial class Verification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "VerifiedCharacter",
                table: "UsersSettings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerifiedCharacter",
                table: "UsersSettings");
        }
    }
}
