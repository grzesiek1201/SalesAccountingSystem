using AccountingSystem.Application.DTOs.Orders;
using AccountingSystem.Application.Helpers.Snapshots;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Orders;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using AccountingSystem.Application.Mappers;

namespace AccountingSystem.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;
        private readonly INumberSequenceService _numberSequenceService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly OrderResponseMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            OrderValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<OrderService> logger,
            INumberSequenceService numberSequenceService,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            OrderResponseMapper mapper)
        {
            _orderRepository = orderRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _numberSequenceService = numberSequenceService;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        // ================= ADD =================

        public OrderAddResponse AddOrder(CreateOrderRequest request)
        {
            _logger.LogInformation("AddOrder start. CustomerId: {CustomerId}", request.CustomerId);

            var customer = _customerRepository.GetById(request.CustomerId);
            if (customer == null)
                return new OrderAddResponse { Result = OrderAddResult.InvalidData };

            var productIds = request.Items?
                .Select(i => i.ProductId)
                .ToList() ?? new List<int>();

            var products = _productRepository
                .GetByIds(productIds)
                .ToDictionary(p => p.Id);

            var order = new Order
            {
                CustomerId = request.CustomerId,
                Status = OrderStatus.Draft,
                DateCreated = DateTime.UtcNow,
                OrderNumber = _numberSequenceService.GetNext(DocumentType.Order)
            };

            order.ApplyCustomerSnapshot(customer);

            var domainItems = request.Items?
                .Select(x => new OrderItem
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    DiscountPercent = x.DiscountPercent
                })
                .ToList() ?? new List<OrderItem>();

            order.Items = ItemSnapshotHelper.SnapshotOrderItems(
                domainItems,
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

        public OrderEditResponse EditOrder(UpdateOrderRequest request)
        {
            _logger.LogInformation("EditOrder start. Id: {Id}", request.Id);

            var existing = _orderRepository.GetById(request.Id);

            if (existing == null)
                return new OrderEditResponse { Result = OrderEditResult.NotFound };

            if (existing.IsOrderArchived)
                return new OrderEditResponse { Result = OrderEditResult.OrderArchived };

            if (request.Items != null && request.Items.Any())
            {
                var domainItems = request.Items
                    .Select(x => new OrderItem
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        DiscountPercent = x.DiscountPercent
                    })
                    .ToList();

                var productIds = domainItems?.Select(i => i.ProductId).ToList() ?? new List<int>();

                var products = _productRepository
                    .GetByIds(productIds)
                    .ToDictionary(p => p.Id);

                existing.Items = ItemSnapshotHelper.SnapshotOrderItems(
                    domainItems,
                    products);
            }

            if (request.CustomerId != 0 && request.CustomerId != existing.CustomerId)
            {
                var customer = _customerRepository.GetById(request.CustomerId);
                if (customer == null)
                    return new OrderEditResponse { Result = OrderEditResult.InvalidData };

                existing.ApplyCustomerSnapshot(customer);
            }

            if (request.Status != default)
                existing.Status = request.Status;

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

        public OrderStatusResponse ChangeOrderStatus(int id, StatusOrderRequest request)
        {
            _logger.LogInformation("ChangeStatus {Id} -> {Status}", id, request);

            var order = _orderRepository.GetById(id);

            if (order == null)
                return new OrderStatusResponse { Result = OrderStatusResult.NotFound };

            if (order.IsOrderArchived)
                return new OrderStatusResponse { Result = OrderStatusResult.InvalidOperation };

            order.Status = request.Status;

            _orderRepository.Update(order);
            _unitOfWork.Save();

            return new OrderStatusResponse { Result = OrderStatusResult.Success };
        }

        // ================= READ =================

        public List<OrderResponse> GetAllOrders()
        {
            _logger.LogInformation("GetAllOrders");

            return _orderRepository.GetAll()
                .Select(o => _mapper.Map(o))
                .ToList();
        }

        public OrderResponse? FindOrder(int id)
        {
            _logger.LogInformation("FindOrder: {OrderId}", id);

            var order = _orderRepository.GetById(id);
            if (order == null)
                return null;

            return _mapper.Map(order);
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