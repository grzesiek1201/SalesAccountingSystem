using AccountingSystem.Application.Repositories;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Customer> GetAll()
            => _context.Customers.AsNoTracking().ToList();

        public Customer? GetById(int id)
            => _context.Customers.FirstOrDefault(x => x.Id == id);

        public void Add(Customer customer)
            => _context.Customers.Add(customer);

        public void Update(Customer customer)
            => _context.Customers.Update(customer);
    }
}