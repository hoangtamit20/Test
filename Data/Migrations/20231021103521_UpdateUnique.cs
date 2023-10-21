using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetShop.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductTranslation_LanguageId",
                table: "ProductTranslation");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "Payment",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTranslation_LanguageId_ProductId",
                table: "ProductTranslation",
                columns: new[] { "LanguageId", "ProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductTranslation_LanguageId_ProductId",
                table: "ProductTranslation");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "Payment",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTranslation_LanguageId",
                table: "ProductTranslation",
                column: "LanguageId");
        }
    }
}
