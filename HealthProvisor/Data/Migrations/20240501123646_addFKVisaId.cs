using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthProvisor.Data.Migrations
{
    public partial class addFKVisaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VisaId",
                table: "Consultations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_VisaId",
                table: "Consultations",
                column: "VisaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultations_Visas_VisaId",
                table: "Consultations",
                column: "VisaId",
                principalTable: "Visas",
                principalColumn: "VisaId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultations_Visas_VisaId",
                table: "Consultations");

            migrationBuilder.DropIndex(
                name: "IX_Consultations_VisaId",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "VisaId",
                table: "Consultations");
        }
    }
}
