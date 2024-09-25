using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCultivationOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HarvestDate",
                table: "Crops");

            migrationBuilder.DropColumn(
                name: "SowingDate",
                table: "Crops");

            migrationBuilder.AlterColumn<string>(
                name: "Coordinates",
                table: "Fields",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "CultivationOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CropId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CultivationOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CultivationOperations_Crops_CropId",
                        column: x => x.CropId,
                        principalTable: "Crops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CultivationOperations_CropId",
                table: "CultivationOperations",
                column: "CropId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CultivationOperations");

            migrationBuilder.AlterColumn<string>(
                name: "Coordinates",
                table: "Fields",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HarvestDate",
                table: "Crops",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SowingDate",
                table: "Crops",
                type: "datetime2",
                nullable: true);
        }
    }
}
