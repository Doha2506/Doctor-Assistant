using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doctor_Assistant.Migrations
{
    public partial class addRayImageColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RayImage",
                table: "rays",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RayImage",
                table: "rays");
        }
    }
}
