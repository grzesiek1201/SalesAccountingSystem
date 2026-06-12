using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Orders;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            OrderValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public OrderAddResponse AddOrder(Order order)
        {
            _logger.LogInformation("Starting AddOrder. CustomerId: {CustomerId}", order.Customer?.Id);

            var orders = _orderRepository.GetAll();
            var result = _validator.Validate(order, orders);

            if (!result.IsValid)
            {
                _logger.LogWarning("AddOrder validation failed. Errors: {Errors}", result.Errors);

                return new OrderAddResponse
                {
                    Result = OrderAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _orderRepository.Add(order);
            _unitOfWork.Save();

            _logger.LogInformation("Order added successfully. OrderId: {OrderId}", order.Id);

            return new OrderAddResponse
            {
                Result = OrderAddResult.Success
            };
        }

        public OrderEditResult EditOrder(Order order)
        {
            _logger.LogInformation("Starting EditOrder. OrderId: {OrderId}", order.Id);

            var existing = _orderRepository.GetById(order.Id);

            if (existing == null)
            {
                _logger.LogWarning("Order not found. Id: {OrderId}", order.Id);
                return OrderEditResult.NotFound;
            }

            if (existing.IsOrderArchived)
            {
                _logger.LogWarning("Attempt to edit archived order. Id: {OrderId}", order.Id);
                return OrderEditResult.OrderArchived;
            }

            var otherOrders = _orderRepository
                .GetAll()
                .Where(x => x.Id != order.Id)
                .ToList();

            var result = _validator.Validate(order, otherOrders);

            if (!result.IsValid)
            {
                _logger.LogWarning("EditOrder validation failed. Id: {OrderId}, Errors: {Errors}",
                    order.Id, result.Errors);

                return OrderEditResult.InvalidData;
            }

            existing.Status = order.Status;
            existing.Customer = order.Customer;

            _orderRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Order edited successfully. Id: {OrderId}", order.Id);

            return OrderEditResult.Success;
        }

        public List<Order> GetAllOrders()
        {
            _logger.LogInformation("Fetching all orders");
            return _orderRepository.GetAll();
        }

        public Order? FindOrder(int id)
        {
            _logger.LogInformation("Finding order by Id: {OrderId}", id);
            return _orderRepository.GetById(id);
        }

        public ArchiveOrderResult ArchiveOrder(int id)
        {
            _logger.LogInformation("Archiving order. Id: {OrderId}", id);

            var existing = _orderRepository.GetById(id);

            if (existing == null)
            {
                _logger.LogWarning("Order not found for archive. Id: {OrderId}", id);
                return ArchiveOrderResult.NotFound;
            }

            existing.IsOrderArchived = true;

            _orderRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Order archived successfully. Id: {OrderId}", id);

            return ArchiveOrderResult.Success;
        }
    }
}