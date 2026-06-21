using AccountingSystem.Domain.Entities;

namespace AccountingSystem.API.DTOs.Products
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }
}
