using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Orders
{
    public class UpdateOrderRequest
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public List<UpdateOrderItemRequest> Items { get; set; }
    }
}
