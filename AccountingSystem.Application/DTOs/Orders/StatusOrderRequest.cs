using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Orders
{
    public class StatusOrderRequest
    {
        public OrderStatus Status { get; set; }
    }
}
