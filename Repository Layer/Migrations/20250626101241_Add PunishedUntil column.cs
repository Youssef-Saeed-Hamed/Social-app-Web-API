using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository_Layer.Migrations
{
    public partial class AddPunishedUntilcolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PunishedUntil",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PunishedUntil",
                table: "AspNetUsers");
        }
    }
}
