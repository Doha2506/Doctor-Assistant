using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doctor_Assistant.Migrations
{
    public partial class updateGenderToIsMale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "strokeDisease");

            migrationBuilder.AddColumn<bool>(
                name: "IsMale",
                table: "strokeDisease",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMale",
                table: "strokeDisease");

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "strokeDisease",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
