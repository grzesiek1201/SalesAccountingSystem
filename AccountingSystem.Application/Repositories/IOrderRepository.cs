using System.Collections.Generic;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Repositories
{
    public interface IOrderRepository
    {
        List<Order> GetAll();
        Order? GetById(int id);
        void Add(Order order);
    }
}