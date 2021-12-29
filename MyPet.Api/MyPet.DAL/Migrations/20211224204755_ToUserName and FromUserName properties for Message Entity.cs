using Microsoft.EntityFrameworkCore.Migrations;

namespace MyPet.DAL.Migrations
{
    public partial class ToUserNameandFromUserNamepropertiesforMessageEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromUserName",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToUserName",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromUserName",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ToUserName",
                table: "Messages");
        }
    }
}
