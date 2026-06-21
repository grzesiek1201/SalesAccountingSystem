namespace AccountingSystem.API.DTOs.Quotations
{
    public class CreateQuotationRequest
    {
        public int CustomerId { get; set; }
        public List<CreateQuotationItemRequest> Items { get; set; }
    }
}
