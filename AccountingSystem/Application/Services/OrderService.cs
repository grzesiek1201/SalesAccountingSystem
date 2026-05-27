using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Validation.Orders;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using AccountingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    internal class OrderService
    {
        private readonly AppDbContext _context;
        private readonly OrderValidator _validator;

        public OrderService(AppDbContext context, OrderValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public OrderAddResponse AddOrder(Order order)
        {
            var orders = _context.Orders.ToList();

            var result = _validator.Validate(order, orders);

            if (!result.IsValid)
            {
                return new OrderAddResponse
                {
                    Result = OrderAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            return new OrderAddResponse
            {
                Result = OrderAddResult.Success
            };
        }

        public OrderEditResult EditOrder(Order order)
        {
            var existing = _context.Orders
                .FirstOrDefault(x => x.Id == order.Id);

            if (existing == null)
                return OrderEditResult.NotFound;

            if (existing.IsOrderArchived)
                return OrderEditResult.OrderArchived;

            var otherOrders = _context.Orders
                .Where(x => x.Id != order.Id)
                .ToList();

            var result = _validator.Validate(order, otherOrders);

            if (!result.IsValid)
                return OrderEditResult.InvalidData;

            existing.Status = order.Status;
            existing.Customer = order.Customer;

            _context.SaveChanges();

            return OrderEditResult.Success;
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders.ToList();
        }

        public Order FindOrder(int id)
        {
            return _context.Orders
                .FirstOrDefault(x => x.Id == id);
        }

        public ArchiveOrderResult ArchiveOrder(int id)
        {
            var existing = _context.Orders
                .FirstOrDefault(x => x.Id == id);

            if (existing == null)
            {
                return ArchiveOrderResult.NotFound;
            }

            existing.IsOrderArchived = true;

            _context.SaveChanges();

            return ArchiveOrderResult.Success;
        }
    }
}

