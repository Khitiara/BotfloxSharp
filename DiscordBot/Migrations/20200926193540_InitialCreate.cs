using Microsoft.EntityFrameworkCore.Migrations;

namespace Botflox.Bot.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildsSettings",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(nullable: false),
                    CommandPrefix = table.Column<string>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildsSettings", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "UsersSettings",
                columns: table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    MainCharacter = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersSettings", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersSettings_MainCharacter",
                table: "UsersSettings",
                column: "MainCharacter",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildsSettings");

            migrationBuilder.DropTable(
                name: "UsersSettings");
        }
    }
}
