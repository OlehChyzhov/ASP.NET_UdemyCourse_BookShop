using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookShop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedCompanyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "company",
                columns: new[] { "id", "city", "name", "phone_number", "postal_code", "state", "street_address" },
                values: new object[,]
                {
                    { 1, "Tech City", "Tech Solution", "666999000", "12121", "IL", "123 Tech St" },
                    { 2, "Vid City", "Vivid Books", "777888555", "34343", "IL", "999 Vid St" },
                    { 3, "Lala land", "Readers Club", "222999333", "99999", "UG", "999 Main St" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "company",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "company",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "company",
                keyColumn: "id",
                keyValue: 3);
        }
    }
}
