using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMethodFromFertilizationAndPlantProtection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Method",
                table: "PlantProtection");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "Fertilizations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "PlantProtection",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "Fertilizations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
