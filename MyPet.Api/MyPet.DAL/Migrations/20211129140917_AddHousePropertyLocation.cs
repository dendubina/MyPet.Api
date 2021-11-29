using Microsoft.EntityFrameworkCore.Migrations;

namespace MyPet.DAL.Migrations
{
    public partial class AddHousePropertyLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "House",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "House",
                table: "Locations");
        }
    }
}
