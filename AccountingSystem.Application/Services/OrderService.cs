using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Mappers;
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
        private readonly NumberSequenceService _numberSequenceService;
        private readonly InvoiceService _invoiceService;
        private readonly QuotationToOrderMapper _mapperQ;
        private readonly OrderToInvoiceMapper _mapperI;

        public OrderService(
            IOrderRepository orderRepository,
            OrderValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<OrderService> logger,
            NumberSequenceService numberSequenceService,
            InvoiceService invoiceService,
            OrderToInvoiceMapper mapperI,
            QuotationToOrderMapper mapperQ)
        {
            _orderRepository = orderRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _numberSequenceService = numberSequenceService;
            _invoiceService = invoiceService;
            _mapperI = mapperI;
            _mapperQ = mapperQ;
        }

        // ================= ADD =================

        public OrderAddResponse AddOrder(Order order)
        {
            _logger.LogInformation("AddOrder start. CustomerId: {CustomerId}", order.CustomerId);

            order.OrderNumber =
                _numberSequenceService.GetNext(DocumentType.Order);

            var validation = _validator.Validate(
                order,
                _orderRepository.GetAll());

            if (!validation.IsValid)
            {
                _logger.LogWarning("AddOrder validation failed: {Errors}", validation.Errors);

                return new OrderAddResponse
                {
                    Result = OrderAddResult.InvalidData,
                    Errors = validation.Errors
                };
            }

            _orderRepository.Add(order);
            _unitOfWork.Save();

            _logger.LogInformation("Order created manually. Id: {OrderId}", order.Id);

            return new OrderAddResponse
            {
                Result = OrderAddResult.Success
            };
        }

        // ================= CREATE FROM QUOTATION =================

        public OrderAddResponse CreateFromQuotation(Quotation quotation)
        {
            _logger.LogInformation("CreateFromQuotation start. QuotationId: {QuotationId}", quotation?.Id);

            if (quotation == null)
            {
                _logger.LogWarning("Quotation is null");
                return new OrderAddResponse { Result = OrderAddResult.InvalidData };
            }
            
            var order = _mapperQ.Map(quotation);

            if (order == null)
            {
                _logger.LogWarning("Mapper returned null for QuotationId: {QuotationId}", quotation.Id);
                return new OrderAddResponse { Result = OrderAddResult.InvalidData };
            }

            _logger.LogInformation(
                "Mapped quotation to order. CustomerId: {CustomerId}, Items: {Count}",
                order.CustomerId,
                order.Items?.Count ?? 0);
                order.OrderNumber =
            _numberSequenceService.GetNext(DocumentType.Order);

            var validation = _validator.Validate(order, _orderRepository.GetAll());

            if (!validation.IsValid)
            {
                _logger.LogWarning(
                    "Order from quotation invalid. QuotationId: {QuotationId}, Errors: {Errors}",
                    quotation.Id,
                    validation.Errors);

                return new OrderAddResponse
                {
                    Result = OrderAddResult.InvalidData,
                    Errors = validation.Errors
                };
            }

            _orderRepository.Add(order);
            _unitOfWork.Save();

            _logger.LogInformation(
                "Order created from quotation SUCCESS. OrderId: {OrderId}",
                order.Id);

            return new OrderAddResponse
            {
                Result = OrderAddResult.Success
            };
        }

        // ================= EDIT =================

        public OrderEditResult EditOrder(Order order)
        {
            _logger.LogInformation("EditOrder start. OrderId: {OrderId}", order.Id);

            var existing = _orderRepository.GetById(order.Id);

            if (existing == null)
            {
                _logger.LogWarning("Order not found: {OrderId}", order.Id);
                return OrderEditResult.NotFound;
            }

            if (existing.IsOrderArchived)
            {
                _logger.LogWarning("Order archived: {OrderId}", order.Id);
                return OrderEditResult.OrderArchived;
            }

            var validation = _validator.Validate(order,
                _orderRepository.GetAll().Where(x => x.Id != order.Id).ToList());

            if (!validation.IsValid)
            {
                _logger.LogWarning("EditOrder invalid: {Errors}", validation.Errors);
                return OrderEditResult.InvalidData;
            }

            existing.Status = order.Status;
            existing.CustomerId = order.CustomerId;

            _orderRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Order edited successfully: {OrderId}", order.Id);

            return OrderEditResult.Success;
        }

        // ================= CONVERT =================

        public ConvertOrderResult ConvertToInvoice(int id)
        {
            _logger.LogInformation("ConvertToInvoice start. Id: {Id}", id);

            var order = _orderRepository.GetById(id);

            if (order == null)
            {
                _logger.LogWarning("Order not found: {Id}", id);
                return ConvertOrderResult.NotFound;
            }

            if (order.IsOrderArchived)
            {
                _logger.LogWarning("Order archived: {Id}", id);
                return ConvertOrderResult.InvalidData;
            }

            if (order.Status != OrderStatus.Completed)
            {
                _logger.LogWarning("Order not completed: {Id}", id);
                return ConvertOrderResult.InvalidData;
            }

            _logger.LogInformation("Mapping order -> invoice: {Id}", id);

            var invoice = _mapperI.Map(order);

            var result = _invoiceService.AddInvoice(invoice);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Invoice creation failed: {Id}", id);
                return ConvertOrderResult.InvalidData;
            }

            order.Status = OrderStatus.ConvertedToInvoice;

            _orderRepository.Update(order);
            _unitOfWork.Save();

            _logger.LogInformation("Convert success: {Id}", id);

            return ConvertOrderResult.Success;
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