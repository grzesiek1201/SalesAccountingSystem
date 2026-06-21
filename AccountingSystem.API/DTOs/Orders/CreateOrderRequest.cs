namespace AccountingSystem.API.DTOs.Orders
{
    public class CreateOrderRequest
    {
        public int CustomerId { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; }
    }
}
