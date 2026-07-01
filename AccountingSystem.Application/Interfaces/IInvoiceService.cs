using AccountingSystem.Application.DTOs.Invoices;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.Interfaces
{
    public interface IInvoiceService
    {
        InvoiceAddResponse AddInvoice(CreateInvoiceRequest request);
        InvoiceEditResponse EditInvoice(UpdateInvoiceRequest request);
        List<InvoiceResponse> GetAllInvoices();
        InvoiceStatusResponse ChangeInvoiceStatus(int id, StatusInvoiceRequest request);
        ArchiveInvoiceResult ArchiveInvoice(int id);
    }
}
