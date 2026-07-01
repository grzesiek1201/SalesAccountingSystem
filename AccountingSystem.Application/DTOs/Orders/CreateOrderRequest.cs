namespace AccountingSystem.Application.DTOs.Orders
{
    public class CreateOrderRequest
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string OrderNumber { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; }
    }
}
