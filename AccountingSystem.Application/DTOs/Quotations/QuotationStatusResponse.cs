using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Quotations
{
    public class QuotationStatusResponse
    {
        public QuotationStatusResult Result { get; set; }

        public List<QuotationValidationError> Errors { get; set; } = new List<QuotationValidationError>();

        public bool IsSuccess => Result == QuotationStatusResult.Success;
    }
}
