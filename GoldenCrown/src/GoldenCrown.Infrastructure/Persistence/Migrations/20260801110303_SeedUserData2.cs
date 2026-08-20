using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoldenCrown.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserData2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password" },
                values: new object[,]
                {
                    { 1, "admin", "Administrator", "admin123" },
                    { 2, "ivan", "Ivan Petrov", "ivan123" },
                    { 3, "maria", "Maria Smirnova", "maria123" },
                    { 4, "alex", "Alex Kuznetsov", "alex123" },
                    { 5, "elena", "Elena Sokolova", "elena123" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password" },
                values: new object[,]
                {
                    { -5, "elena", "Elena Sokolova", "elena123" },
                    { -4, "alex", "Alex Kuznetsov", "alex123" },
                    { -3, "maria", "Maria Smirnova", "maria123" },
                    { -2, "ivan", "Ivan Petrov", "ivan123" },
                    { -1, "admin", "Administrator", "admin123" }
                });
        }
    }
}
