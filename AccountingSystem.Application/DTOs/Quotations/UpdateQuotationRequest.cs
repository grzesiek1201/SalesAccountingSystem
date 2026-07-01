using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Quotations
{
    public class UpdateQuotationRequest
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }

        public QuotationStatus Status { get; set; }
        public List<UpdateQuotationItemRequest> Items { get; set; }
    }
}
