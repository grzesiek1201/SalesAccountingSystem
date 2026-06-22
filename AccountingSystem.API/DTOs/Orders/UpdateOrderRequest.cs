namespace AccountingSystem.API.DTOs.Orders
{
    public class UpdateOrderRequest
    {
        public int CustomerId { get; set; }
        public List<UpdateOrderItemRequest> Items { get; set; }
    }
}
