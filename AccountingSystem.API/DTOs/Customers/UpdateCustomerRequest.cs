namespace AccountingSystem.API.DTOs.Customers
{
    public class UpdateCustomerRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }
}
