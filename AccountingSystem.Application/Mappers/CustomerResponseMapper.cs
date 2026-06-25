using AccountingSystem.Application.DTOs.Customers;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Mappers
{
    public class CustomerResponseMapper
    {
        public CustomerResponse Map(Customer c)
        {
            return new CustomerResponse
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Street = c.Street,
                City = c.City,
                ZipCode = c.ZipCode
            };
        }
    }
}
