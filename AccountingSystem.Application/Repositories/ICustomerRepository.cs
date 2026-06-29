using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Repositories
{
    public interface ICustomerRepository
    {
        Customer? GetById(int id);
        List<Customer> GetAll();
        void Add(Customer customer);
        void Update(Customer customer);
    }
}