namespace AccountingSystem.Domain.Entities
{
    public class ProductCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }

        public bool IsActive { get; set; }
    }
}
