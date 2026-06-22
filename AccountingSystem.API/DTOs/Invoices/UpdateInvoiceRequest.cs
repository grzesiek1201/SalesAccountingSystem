namespace AccountingSystem.API.DTOs.Invoices
{
    public class UpdateInvoiceRequest
    {
        public int CustomerId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public List<UpdateInvoiceItemRequest> Items { get; set; }
    }
}
