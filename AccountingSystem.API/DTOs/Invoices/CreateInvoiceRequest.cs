using AccountingSystem.API.DTOs.Orders;

namespace AccountingSystem.API.DTOs.Invoices
{
    public class CreateInvoiceRequest
    {
        public int CustomerId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public List<CreateInvoiceItemRequest> Items { get; set; }
    }
}
