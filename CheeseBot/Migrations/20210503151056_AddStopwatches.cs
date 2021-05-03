using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CheeseBot.Migrations
{
    public partial class AddStopwatches : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stopwatches",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "numeric(20,0)", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stopwatches", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stopwatches");
        }
    }
}
