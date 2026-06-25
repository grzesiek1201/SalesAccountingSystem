using AccountingSystem.Application.DTOs.Customers;
using AccountingSystem.Domain.Enums;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Interfaces
{
    public interface ICustomerService
    {
        CustomerAddResponse AddCustomer(CreateCustomerRequest request);
        CustomerEditResponse EditCustomer(UpdateCustomerRequest request);

        List<CustomerResponse> GetAllCustomers();
        CustomerResponse? GetCustomerById(int id);

        CustomerArchiveResult ArchiveCustomer(int id);
    }
}
