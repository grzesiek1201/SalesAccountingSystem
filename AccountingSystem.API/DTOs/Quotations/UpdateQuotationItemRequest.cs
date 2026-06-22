namespace AccountingSystem.API.DTOs.Quotations
{
    public class UpdateQuotationItemRequest
    {
        public int? Id { get; set; } 
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountPercent { get; set; }
        public int Position { get; set; }
    }
}
