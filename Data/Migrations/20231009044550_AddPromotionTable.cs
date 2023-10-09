using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetShop.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "ProductCategoryIds",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "ProductIds",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "PaymentRefId",
                table: "Payment");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Promotion",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Promotion",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<int>(
                name: "DiscountType",
                table: "Promotion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountValue",
                table: "Promotion",
                type: "decimal(19,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "PromotionCategory",
                columns: table => new
                {
                    PromotionId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionCategory", x => new { x.PromotionId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_PromotionCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionCategory_Promotion_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionProduct",
                columns: table => new
                {
                    PromotionId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionProduct", x => new { x.PromotionId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_PromotionProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionProduct_Promotion_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCategory_CategoryId",
                table: "PromotionCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionProduct_ProductId",
                table: "PromotionProduct",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionCategory");

            migrationBuilder.DropTable(
                name: "PromotionProduct");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "DiscountValue",
                table: "Promotion");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Promotion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Promotion",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "DiscountAmount",
                table: "Promotion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiscountPercent",
                table: "Promotion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductCategoryIds",
                table: "Promotion",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductIds",
                table: "Promotion",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentRefId",
                table: "Payment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
