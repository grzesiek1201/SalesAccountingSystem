using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Quotations
{
    public class QuotationEditResponse
    {
        public QuotationEditResult Result { get; set; }

        public List<QuotationValidationError> Errors { get; set; } = new List<QuotationValidationError>();

        public bool IsSuccess => Result == QuotationEditResult.Success;
    }
}
