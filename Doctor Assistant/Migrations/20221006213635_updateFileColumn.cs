using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doctor_Assistant.Migrations
{
    public partial class updateFileColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RayImage",
                table: "rays");

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "rays",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "rays");

            migrationBuilder.AddColumn<byte[]>(
                name: "RayImage",
                table: "rays",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
