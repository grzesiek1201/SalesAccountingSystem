using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Customers
{
    public class CustomerEditResponse
    {
        public CustomerEditResult Result { get; set; }

        public List<CustomerValidationError> Errors { get; set; } = new List<CustomerValidationError>();

        public bool IsSuccess => Result == CustomerEditResult.Success;
    }
}
