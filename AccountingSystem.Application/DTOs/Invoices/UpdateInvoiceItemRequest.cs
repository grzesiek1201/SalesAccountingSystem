namespace AccountingSystem.Application.DTOs.Invoices
{
    public class UpdateInvoiceItemRequest
    {
        public int? Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountPercent { get; set; }
        public int Position { get; set; }
    }
}
