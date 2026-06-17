using AccountingSystem.Application.Repositories;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Order> GetAll()
             => _context.Orders
                 .Include(x => x.Items)
                 .Include(x => x.Customer)
                 .AsNoTracking()
                 .ToList();

        public Order? GetById(int id)
            => _context.Orders
                .Include(x => x.Items)
                .Include(x => x.Customer)
                .FirstOrDefault(x => x.Id == id);

        public void Add(Order order)
            => _context.Orders.Add(order);

        public void Update(Order order)
            => _context.Orders.Update(order);
    }
}