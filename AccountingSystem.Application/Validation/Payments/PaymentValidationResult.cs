namespace AccountingSystem.Application.Validation.Payments
{
    public class PaymentValidationResult
    {
        public List<PaymentValidationError> Errors { get; set; } = new List<PaymentValidationError>();

        public bool IsValid => Errors.Count == 0;
    }
}
