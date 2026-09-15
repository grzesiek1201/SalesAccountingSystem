using AccountingSystem.Application.DTOs.Quotations;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.Interfaces
{
    public interface IQuotationService
    {
        QuotationAddResponse AddQuotation(CreateQuotationRequest request);

        QuotationEditResponse EditQuotation(UpdateQuotationRequest request);

        List<QuotationResponse> GetAllQuotations();

        QuotationResponse? FindQuotation(int id);




        QuotationStatusResponse SendQuotation(int id);

        QuotationStatusResponse AcceptQuotation(int id);

        QuotationStatusResponse RejectQuotation(int id);


        QuotationArchiveResult ArchiveQuotation(int id);
    }
}