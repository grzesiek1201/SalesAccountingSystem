using AccountingSystem.Application.DTOs.Customers;

namespace AccountingSystem.Application.DTOs.Quotations
{
    public class QuotationResponse
    {
        public int Id { get; set; }
        public string QuotationNumber { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }

        public CustomerResponse Customer { get; set; }

        public List<QuotationItemResponse> Items { get; set; }
    }
}
