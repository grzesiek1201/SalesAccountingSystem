using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Customers
{
    public class CustomerAddResponse
    {
        public CustomerAddResult Result { get; set; }

        public List<CustomerValidationError> Errors { get; set; } = new List<CustomerValidationError>();

        public bool IsSuccess => Result == CustomerAddResult.Success;
    }
}
