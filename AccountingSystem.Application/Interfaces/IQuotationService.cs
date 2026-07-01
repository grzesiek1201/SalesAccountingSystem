using AccountingSystem.Application.DTOs.Quotations;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.Interfaces
{
    public interface IQuotationService
    {
        QuotationAddResponse AddQuotation(CreateQuotationRequest request);
        QuotationEditResponse EditQuotation(UpdateQuotationRequest request);
        List<QuotationResponse> GetAllQuotations();
        QuotationStatusResponse ChangeQuotationStatus(int id, StatusQuotationRequest request);
        QuotationArchiveResult ArchiveQuotation(int id);
    }
}
