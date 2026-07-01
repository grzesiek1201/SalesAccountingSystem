using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Orders
{
    public class OrderAddResponse
    {
        public OrderAddResult Result { get; set; }

        public List<OrderValidationError> Errors { get; set; } = new List<OrderValidationError>();

        public bool IsSuccess => Result == OrderAddResult.Success;
    }
}
