namespace AccountingSystem.Application.DTOs.Invoices
{
    public class CreateInvoiceRequest
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string InvoiceNumber { get; set; }
        public List<CreateInvoiceItemRequest> Items { get; set; }
    }
}
