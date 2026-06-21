using AccountingSystem.Domain.Entities;

namespace AccountingSystem.API.DTOs.Products
{
    public class UpdateProductRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }
}
