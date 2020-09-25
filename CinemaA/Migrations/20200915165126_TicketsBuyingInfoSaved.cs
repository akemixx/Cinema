using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CinemaA.Migrations
{
    public partial class TicketsBuyingInfoSaved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuyerEmail",
                table: "Tickets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BuyingDateTime",
                table: "Tickets",
                nullable: true,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyerEmail",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "BuyingDateTime",
                table: "Tickets");
        }
    }
}
