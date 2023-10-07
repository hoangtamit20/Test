using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetShop.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnForOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "Payment",
                newName: "PaymentCurrency");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Payment",
                newName: "PaymentContent");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Order",
                type: "decimal(19,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "PaymentCurrency",
                table: "Payment",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "PaymentContent",
                table: "Payment",
                newName: "Content");
        }
    }
}
