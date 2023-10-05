using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetShop.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Order_PaymentId",
                table: "Payment");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "Payment",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_PaymentId",
                table: "Payment",
                newName: "IX_Payment_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Order_OrderId",
                table: "Payment",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Order_OrderId",
                table: "Payment");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Payment",
                newName: "PaymentId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_OrderId",
                table: "Payment",
                newName: "IX_Payment_PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Order_PaymentId",
                table: "Payment",
                column: "PaymentId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
