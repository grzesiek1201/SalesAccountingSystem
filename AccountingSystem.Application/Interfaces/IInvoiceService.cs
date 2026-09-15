using AccountingSystem.Application.DTOs.Invoices;
using AccountingSystem.Domain.Enums;

public interface IInvoiceService
{
    InvoiceAddResponse AddInvoice(CreateInvoiceRequest request);
    InvoiceEditResponse EditInvoice(UpdateInvoiceRequest request);

    List<InvoiceResponse> GetAllInvoices();
    InvoiceResponse? FindInvoice(int id);
    InvoiceAddResponse CreateInvoiceFromOrder(int orderId);

    InvoiceStatusResponse IssueInvoice(int id);
    InvoiceStatusResponse CancelInvoice(int id);

    ArchiveInvoiceResult ArchiveInvoice(int id);
}