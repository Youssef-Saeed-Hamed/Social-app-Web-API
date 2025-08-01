using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository_Layer.Migrations
{
    public partial class AddCounterforBullying : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "counterOfBullying",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 5);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "counterOfBullying",
                table: "AspNetUsers");
        }
    }
}
