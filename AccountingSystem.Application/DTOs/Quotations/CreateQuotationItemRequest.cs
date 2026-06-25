namespace AccountingSystem.Application.DTOs.Quotations
{
    public class CreateQuotationItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountPercent { get; set; }
        public int Position { get; set; }
    }
}
