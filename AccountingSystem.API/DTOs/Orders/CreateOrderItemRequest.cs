namespace AccountingSystem.API.DTOs.Orders
{
    public class CreateOrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountPercent { get; set; }
    }
}
