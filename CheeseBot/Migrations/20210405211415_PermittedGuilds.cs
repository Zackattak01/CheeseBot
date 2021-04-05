using Microsoft.EntityFrameworkCore.Migrations;

namespace CheeseBot.Migrations
{
    public partial class PermittedGuilds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_permitted",
                table: "guild_settings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_permitted",
                table: "guild_settings");
        }
    }
}
