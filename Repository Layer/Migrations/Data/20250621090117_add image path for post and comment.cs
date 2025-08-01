using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository_Layer.Migrations.Data
{
    public partial class addimagepathforpostandcomment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imagePath",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "imagePath",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imagePath",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "imagePath",
                table: "Comments");
        }
    }
}
