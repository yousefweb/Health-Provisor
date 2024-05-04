using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthProvisor.Data.Migrations
{
    public partial class addPropStatustoConsultClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Patient_Status",
                table: "Consultations",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Patient_Status",
                table: "Consultations");
        }
    }
}
