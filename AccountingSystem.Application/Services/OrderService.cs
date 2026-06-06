using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Orders;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IOrderRepository orderRepository,
            OrderValidator validator,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public OrderAddResponse AddOrder(Order order)
        {
            var orders = _orderRepository.GetAll();

            var result = _validator.Validate(order, orders);

            if (!result.IsValid)
            {
                return new OrderAddResponse
                {
                    Result = OrderAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _orderRepository.Add(order);
            _unitOfWork.Save();

            return new OrderAddResponse
            {
                Result = OrderAddResult.Success
            };
        }

        public OrderEditResult EditOrder(Order order)
        {
            var existing = _orderRepository.GetById(order.Id);

            if (existing == null)
                return OrderEditResult.NotFound;

            if (existing.IsOrderArchived)
                return OrderEditResult.OrderArchived;

            var otherOrders = _orderRepository
                .GetAll()
                .Where(x => x.Id != order.Id)
                .ToList();

            var result = _validator.Validate(order, otherOrders);

            if (!result.IsValid)
                return OrderEditResult.InvalidData;

            existing.Status = order.Status;
            existing.Customer = order.Customer;

            _orderRepository.Update(existing);
            _unitOfWork.Save();

            return OrderEditResult.Success;
        }

        public List<Order> GetAllOrders()
        {
            return _orderRepository.GetAll();
        }

        public Order? FindOrder(int id)
        {
            return _orderRepository.GetById(id);
        }

        public ArchiveOrderResult ArchiveOrder(int id)
        {
            var existing = _orderRepository.GetById(id);

            if (existing == null)
                return ArchiveOrderResult.NotFound;

            existing.IsOrderArchived = true;

            _orderRepository.Update(existing);
            _unitOfWork.Save();

            return ArchiveOrderResult.Success;
        }
    }
}