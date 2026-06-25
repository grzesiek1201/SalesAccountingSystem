namespace AccountingSystem.Application.Validation.Customers
{
    public class CustomerValidationResult
    {
        public List<CustomerValidationError> Errors { get; set; } = new List<CustomerValidationError>();
        public bool IsValid => Errors.Count == 0;

    }
}
