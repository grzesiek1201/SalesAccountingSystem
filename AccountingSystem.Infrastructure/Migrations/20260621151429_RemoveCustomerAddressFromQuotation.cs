using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCustomerAddressFromQuotation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerAddress",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CustomerAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerAddress",
                table: "Invoices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerAddress",
                table: "Quotations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerAddress",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
