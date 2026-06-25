namespace AccountingSystem.Application.DTOs.Invoices
{
    public class UpdateInvoiceRequest
    {
        public int CustomerId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public List<UpdateInvoiceItemRequest> Items { get; set; }
    }
}
