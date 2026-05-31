using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Repositories
{
    public interface ICustomerRepository
    {
        List<Customer> GetAll();
        Customer? GetById(int id);
        void Add(Customer customer);
        void Update(Customer customer);
    }
}