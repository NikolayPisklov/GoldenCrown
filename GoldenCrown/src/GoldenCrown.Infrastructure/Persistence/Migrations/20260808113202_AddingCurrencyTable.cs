using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoldenCrown.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingCurrencyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "RU" },
                    { 2, "USD" },
                    { 3, "EUR" },
                    { 4, "JPY" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CurrencyId",
                table: "Accounts",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Currency_Name",
                table: "Currency",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Currency_CurrencyId",
                table: "Accounts",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Currency_CurrencyId",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_CurrencyId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Accounts");

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
    }
}
