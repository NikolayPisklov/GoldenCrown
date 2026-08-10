using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoldenCrown.Database.Migrations
{
    /// <inheritdoc />
    public partial class CorrectCodeNameForRuble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "RUB");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "RU");
        }
    }
}
