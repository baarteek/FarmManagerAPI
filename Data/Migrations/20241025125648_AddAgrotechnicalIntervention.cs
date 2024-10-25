using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAgrotechnicalIntervention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgrotechnicalIntervention",
                table: "PlantProtection",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AgrotechnicalIntervention",
                table: "Fertilizations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AgrotechnicalIntervention",
                table: "CultivationOperations",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgrotechnicalIntervention",
                table: "PlantProtection");

            migrationBuilder.DropColumn(
                name: "AgrotechnicalIntervention",
                table: "Fertilizations");

            migrationBuilder.DropColumn(
                name: "AgrotechnicalIntervention",
                table: "CultivationOperations");
        }
    }
}
