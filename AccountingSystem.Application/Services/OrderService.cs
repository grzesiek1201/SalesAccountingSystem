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

        // ================= ADD MANUAL =================

        public OrderAddResponse AddOrder(Order order)
        {
            _logger.LogInformation("Starting AddOrder. CustomerId: {CustomerId}", order.CustomerId);

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

            return new OrderAddResponse
            {
                Result = OrderAddResult.Success
            };
        }

        // ================= CREATE FROM QUOTATION =================

        public OrderAddResponse CreateFromQuotation(Quotation quotation)
        {
            var order = new Order
            {
                CustomerId = quotation.CustomerId,
                DateCreated = DateTime.Now,
                Status = OrderStatus.Draft,
                QuotationId = quotation.Id,

                Items = quotation.Items.Select(x => new OrderItem
                {
                    Position = x.Position,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    BaseUnitPrice = x.BaseUnitPrice,
                    DiscountPercent = x.DiscountPercent

                }).ToList()
            };


            var validation =
                _validator.Validate(
                    order,
                    _orderRepository.GetAll());


            if (!validation.IsValid)
            {
                return new OrderAddResponse
                {
                    Result = OrderAddResult.InvalidData,
                    Errors = validation.Errors
                };
            }


            _orderRepository.Add(order);

            _unitOfWork.Save();


            return new OrderAddResponse
            {
                Result = OrderAddResult.Success
            };
        }

        // ================= EDIT =================

        public OrderEditResult EditOrder(Order order)
        {
            var existing = _orderRepository.GetById(order.Id);

            if (existing == null)
                return OrderEditResult.NotFound;

            if (existing.IsOrderArchived)
                return OrderEditResult.OrderArchived;

            var otherOrders = _orderRepository.GetAll()
                .Where(x => x.Id != order.Id)
                .ToList();

            var result = _validator.Validate(order, otherOrders);

            if (!result.IsValid)
                return OrderEditResult.InvalidData;

            existing.Status = order.Status;
            existing.CustomerId = order.CustomerId;

            _orderRepository.Update(existing);
            _unitOfWork.Save();

            return OrderEditResult.Success;
        }

        // ================= READ =================

        public List<Order> GetAllOrders()
            => _orderRepository.GetAll();

        public Order? FindOrder(int id)
            => _orderRepository.GetById(id);

        // ================= ARCHIVE =================

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