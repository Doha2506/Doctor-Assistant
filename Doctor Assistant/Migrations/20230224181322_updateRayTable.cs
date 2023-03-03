using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doctor_Assistant.Migrations
{
    public partial class updateRayTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "rays",
                newName: "patientName");

            migrationBuilder.AddColumn<string>(
                name: "patientEmail",
                table: "rays",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "patientEmail",
                table: "rays");

            migrationBuilder.RenameColumn(
                name: "patientName",
                table: "rays",
                newName: "ImageName");
        }
    }
}
