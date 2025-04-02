using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveImageURL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "product_image_url",
                table: "product");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "product_image_url",
                table: "product",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "id",
                keyValue: 1,
                column: "product_image_url",
                value: "");

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "id",
                keyValue: 2,
                column: "product_image_url",
                value: "");

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "id",
                keyValue: 3,
                column: "product_image_url",
                value: "");

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "id",
                keyValue: 4,
                column: "product_image_url",
                value: "");

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "id",
                keyValue: 5,
                column: "product_image_url",
                value: "");

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "id",
                keyValue: 6,
                column: "product_image_url",
                value: "");
        }
    }
}
