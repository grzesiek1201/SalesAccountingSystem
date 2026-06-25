namespace AccountingSystem.Application.DTOs.Quotations
{
    public class UpdateQuotationRequest
    {
        public int CustomerId { get; set; }
        public List<UpdateQuotationItemRequest> Items { get; set; }
    }
}
