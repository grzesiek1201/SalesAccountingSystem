using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuotationCustomerSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerAddress",
                table: "Quotations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerCity",
                table: "Quotations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Quotations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerStreet",
                table: "Quotations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerZipCode",
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
                name: "CustomerCity",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerStreet",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerZipCode",
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

            migrationBuilder.AddColumn<string>(
                name: "CustomerCity",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerStreet",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerZipCode",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerAddress",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CustomerCity",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CustomerStreet",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CustomerZipCode",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CustomerAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerCity",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerStreet",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerZipCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerAddress",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CustomerCity",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CustomerStreet",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CustomerZipCode",
                table: "Invoices");
        }
    }
}
