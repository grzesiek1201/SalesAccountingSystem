using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Orders
{
    public class OrderStatusResponse
    {
        public OrderStatusResult Result { get; set; }

        public List<OrderValidationError> Errors { get; set; } = new List<OrderValidationError>();

        public bool IsSuccess => Result == OrderStatusResult.Success;
    }
}
