using Microsoft.EntityFrameworkCore.Migrations;

namespace MyPet.DAL.Migrations
{
    public partial class AddRegionPropertyToLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Region",
                table: "Locations");
        }
    }
}
