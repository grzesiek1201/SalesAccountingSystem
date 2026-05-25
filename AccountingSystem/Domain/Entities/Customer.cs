namespace AccountingSystem.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        public bool IsCustomerArchived { get; set; }

        public bool InDebt { get; set; }
    }
}