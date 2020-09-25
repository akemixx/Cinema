using Microsoft.EntityFrameworkCore.Migrations;

namespace CinemaA.Migrations
{
    public partial class RealName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameOfUser",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "RealName",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RealName",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "NameOfUser",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
