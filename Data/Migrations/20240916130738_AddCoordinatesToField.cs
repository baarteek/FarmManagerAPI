using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCoordinatesToField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Coordinates",
                table: "Fields",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coordinates",
                table: "Fields");
        }
    }
}
