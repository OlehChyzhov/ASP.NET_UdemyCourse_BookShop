using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class OneToManyRelationshipCategoryProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "product",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "id",
                keyValue: 1,
                column: "category_id",
                value: 1);

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "id",
                keyValue: 2,
                column: "category_id",
                value: 2);

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "id",
                keyValue: 3,
                column: "category_id",
                value: 3);

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "id",
                keyValue: 4,
                column: "category_id",
                value: 1);

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "id",
                keyValue: 5,
                column: "category_id",
                value: 2);

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "id",
                keyValue: 6,
                column: "category_id",
                value: 3);

            migrationBuilder.CreateIndex(
                name: "IX_product_category_id",
                table: "product",
                column: "category_id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_category_category_id",
                table: "product",
                column: "category_id",
                principalTable: "category",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_category_category_id",
                table: "product");

            migrationBuilder.DropIndex(
                name: "IX_product_category_id",
                table: "product");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "product");
        }
    }
}
