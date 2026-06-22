namespace AccountingSystem.Application.DTOs.Orders
{
    public class OrderItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal BaseUnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal Total { get; set; }
    }
}
