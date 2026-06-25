using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Quotations
{
    public class QuotationAddResponse
    {
        public QuotationAddResult Result { get; set; }

        public List<QuotationValidationError> Errors { get; set; } = new List<QuotationValidationError>();

        public bool IsSuccess => Result == QuotationAddResult.Success;
    }
}
