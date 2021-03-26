using Microsoft.EntityFrameworkCore.Migrations;

namespace CheeseBot.Migrations
{
    public partial class SupportMultiplePrefixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "GuildSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                table: "GuildSettings",
                type: "text",
                nullable: true);
        }
    }
}
