using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f18e947f-e735-4ae3-b82c-10b33c938943", 0, "e48de910-6666-4688-b9cb-802359e229f1", "user@example.com", true, false, null, "USER@EXAMPLE.COM", "STRING", "AQAAAAIAAYagAAAAEAnJls5VwNh5qHK/l/lUN+ETwr1Wuj0ofhLsc8UR8EYZMvZDsvcKeBzuq3uLbPNrNw==", null, false, "830f02c7-0d57-4d4b-996d-c7cbff5df409", false, "string" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f18e947f-e735-4ae3-b82c-10b33c938943");
        }
    }
}
