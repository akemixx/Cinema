using Microsoft.EntityFrameworkCore.Migrations;

namespace CinemaA.Migrations
{
    public partial class Renewing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShownOnScreen",
                table: "Films",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShownOnScreen",
                table: "Films");
        }
    }
}
