using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Customers
{
    public class CustomerArchiveResponse
    {
        public CustomerArchiveResult Result { get; set; }

        public List<CustomerValidationError> Errors { get; set; } = new List<CustomerValidationError>();

        public bool IsSuccess => Result == CustomerArchiveResult.Success;
    }
}
