using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerEmailToModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Quotations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Invoices");
        }
    }
}
