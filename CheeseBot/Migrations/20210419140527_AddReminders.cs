using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CheeseBot.Migrations
{
    public partial class AddReminders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reminders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    execution_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    user_id = table.Column<ulong>(type: "numeric(20,0)", nullable: false),
                    channel_id = table.Column<ulong>(type: "numeric(20,0)", nullable: false),
                    guild_id = table.Column<ulong>(type: "numeric(20,0)", nullable: false),
                    original_message_id = table.Column<ulong>(type: "numeric(20,0)", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reminders", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reminders");
        }
    }
}
