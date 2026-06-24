using AccountingSystem.Application.DTOs.Order;
using AccountingSystem.Application.Helpers.Snapshots;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Orders;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AccountingSystem.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;
        private readonly INumberSequenceService _numberSequenceService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            OrderValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<OrderService> logger,
            INumberSequenceService numberSequenceService,
            ICustomerRepository customerRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _numberSequenceService = numberSequenceService;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        // ================= ADD =================

        public OrderAddResponse AddOrder(Order order)
        {
            _logger.LogInformation("AddOrder start. CustomerId: {CustomerId}", order.CustomerId);

            var customer = _customerRepository.GetById(order.CustomerId);
            if (customer == null)
                return new OrderAddResponse { Result = OrderAddResult.InvalidData };

            var productIds = order.Items?
                .Select(i => i.ProductId)
                .ToList() ?? new List<int>();

            var products = _productRepository
                .GetByIds(productIds)
                .ToDictionary(p => p.Id);

            order.OrderNumber =
                _numberSequenceService.GetNext(DocumentType.Order);

            order.ApplyCustomerSnapshot(customer);

            order.Items = ItemSnapshotHelper.SnapshotOrderItems(
                order.Items,
                products);

            var validation = _validator.Validate(
                order,
                _orderRepository.GetAll());

            if (!validation.IsValid)
            {
                _logger.LogWarning("AddOrder invalid: {Errors}", validation.Errors);

                return new OrderAddResponse
                {
                    Result = OrderAddResult.InvalidData,
                    Errors = validation.Errors
                };
            }

            _orderRepository.Add(order);
            _unitOfWork.Save();

            _logger.LogInformation("Order created: {Id}", order.Id);

            return new OrderAddResponse
            {
                Result = OrderAddResult.Success
            };
        }

        // ================= EDIT =================

        public OrderEditResponse EditOrder(Order input)
        {
            _logger.LogInformation("EditOrder start. Id: {Id}", input.Id);

            var existing = _orderRepository.GetById(input.Id);

            if (existing == null)
                return new OrderEditResponse { Result = OrderEditResult.NotFound };

            if (existing.IsOrderArchived)
                return new OrderEditResponse { Result = OrderEditResult.OrderArchived };

            var productIds = input.Items?.Select(i => i.ProductId).ToList() ?? new List<int>();

            var products = _productRepository
                .GetByIds(productIds)
                .ToDictionary(p => p.Id);

            if (input.Items != null && input.Items.Any())
            {
                var itemProductIds = input.Items.Select(i => i.ProductId).ToList();

                var productDict = _productRepository
                    .GetByIds(productIds)
                    .ToDictionary(p => p.Id);

                existing.Items = ItemSnapshotHelper.SnapshotOrderItems(
                    input.Items,
                    products);
            }

            if (input.CustomerId != 0 && input.CustomerId != existing.CustomerId)
            {
                var customer = _customerRepository.GetById(input.CustomerId);
                if (customer == null)
                    return new OrderEditResponse { Result = OrderEditResult.InvalidData };

                existing.ApplyCustomerSnapshot(customer);
            }

            if (input.Status != default)
                existing.Status = input.Status;

            var validation = _validator.Validate(
                existing,
                _orderRepository.GetAll().Where(x => x.Id != existing.Id).ToList(),
                isEdit: true);

            if (!validation.IsValid)
                return new OrderEditResponse { Result = OrderEditResult.InvalidData };

            _orderRepository.Update(existing);
            _unitOfWork.Save();

            return new OrderEditResponse
            {
                Result = OrderEditResult.Success
            };
        }

        // ================= STATUS =================

        public OrderStatusResult ChangeOrderStatus(int id, OrderStatus newStatus)
        {
            _logger.LogInformation("ChangeStatus {Id} -> {Status}", id, newStatus);

            var order = _orderRepository.GetById(id);

            if (order == null)
                return OrderStatusResult.NotFound;

            if (order.IsOrderArchived)
                return OrderStatusResult.InvalidOperation;

            order.Status = newStatus;

            _orderRepository.Update(order);
            _unitOfWork.Save();

            return OrderStatusResult.Success;
        }

        // ================= READ =================

        public List<Order> GetAllOrders()
        {
            _logger.LogInformation("GetAllOrders");
            return _orderRepository.GetAll();
        }

        public Order? FindOrder(int id)
        {
            _logger.LogInformation("FindOrder: {OrderId}", id);
            return _orderRepository.GetById(id);
        }

        // ================= ARCHIVE =================

        public ArchiveOrderResult ArchiveOrder(int id)
        {
            _logger.LogInformation("ArchiveOrder: {OrderId}", id);

            var existing = _orderRepository.GetById(id);

            if (existing == null)
            {
                _logger.LogWarning("Order not found for archive. Id: {OrderId}", id);
                return ArchiveOrderResult.NotFound;
            }

            existing.IsOrderArchived = true;

            _orderRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Order archived: {OrderId}", id);

            return ArchiveOrderResult.Success;
        }
    }
}