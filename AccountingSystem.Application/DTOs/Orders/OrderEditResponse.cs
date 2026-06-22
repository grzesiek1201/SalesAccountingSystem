using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Order
{
    public class OrderEditResponse
    {
        public OrderEditResult Result { get; set; }

        public List<OrderValidationError> Errors { get; set; } = new List<OrderValidationError>();

        public bool IsSuccess => Result == OrderEditResult.Success;
    }
}
