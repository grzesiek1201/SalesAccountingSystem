using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductNameToQuotation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "QuotationItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "QuotationItems");
        }
    }
}
