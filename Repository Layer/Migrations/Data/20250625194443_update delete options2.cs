using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository_Layer.Migrations.Data
{
    public partial class updatedeleteoptions2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikePosts_Posts_PostId",
                table: "LikePosts");

            migrationBuilder.DropForeignKey(
                name: "FK_LikePosts_Users_UserId",
                table: "LikePosts");

            migrationBuilder.AddForeignKey(
                name: "FK_LikePosts_Posts_PostId",
                table: "LikePosts",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LikePosts_Users_UserId",
                table: "LikePosts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikePosts_Posts_PostId",
                table: "LikePosts");

            migrationBuilder.DropForeignKey(
                name: "FK_LikePosts_Users_UserId",
                table: "LikePosts");

            migrationBuilder.AddForeignKey(
                name: "FK_LikePosts_Posts_PostId",
                table: "LikePosts",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LikePosts_Users_UserId",
                table: "LikePosts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
