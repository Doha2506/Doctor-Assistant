using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doctor_Assistant.Migrations
{
    public partial class addStrokeDiseaseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "strokeDisease",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Hypertension = table.Column<bool>(type: "bit", nullable: false),
                    HeartDisease = table.Column<bool>(type: "bit", nullable: false),
                    IsMarried = table.Column<bool>(type: "bit", nullable: false),
                    WorkType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResidenceType = table.Column<bool>(type: "bit", nullable: false),
                    AvrGlucoseLevel = table.Column<float>(type: "real", nullable: false),
                    BMI = table.Column<float>(type: "real", nullable: false),
                    SmokingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stroke = table.Column<bool>(type: "bit", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_strokeDisease", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "strokeDisease");
        }
    }
}
