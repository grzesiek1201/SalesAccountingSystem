using AccountingSystem.Application.Mappers;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.Services
{
    public class DocumentConversionService
    {
        private readonly OrderService _orderService;
        private readonly QuotationService _quotationService;
        private readonly InvoiceService _invoiceService;

        private readonly QuotationToOrderMapper _quotationToOrderMapper;
        private readonly OrderToInvoiceMapper _orderToInvoiceMapper;

        public DocumentConversionService(
            OrderService orderService,
            QuotationService quotationService,
            InvoiceService invoiceService,
            QuotationToOrderMapper quotationToOrderMapper,
            OrderToInvoiceMapper orderToInvoiceMapper)
        {
            _orderService = orderService;
            _quotationService = quotationService;
            _invoiceService = invoiceService;
            _quotationToOrderMapper = quotationToOrderMapper;
            _orderToInvoiceMapper = orderToInvoiceMapper;
        }

        // ================= QUOTATION -> ORDER =================

        public ConvertQuotationResult ConvertQuotationToOrder(int quotationId)
        {
            var quotation = _quotationService.FindQuotation(quotationId);

            if (quotation == null)
                return ConvertQuotationResult.InvalidData;

            if (quotation.IsQuotationArchived || quotation.Status != QuotationStatus.Accepted)
                return ConvertQuotationResult.InvalidData;

            var order = _quotationToOrderMapper.Map(quotation);

            if (order == null)
                return ConvertQuotationResult.InvalidData;

            var result = _orderService.AddOrder(order);

            if (!result.IsSuccess)
                return ConvertQuotationResult.InvalidData;

            _quotationService.ChangeQuotationStatus(
                quotationId,
                QuotationStatus.ConvertedToOrder);

            return ConvertQuotationResult.Success;
        }

        // ================= ORDER -> INVOICE =================

        public ConvertOrderResult ConvertOrderToInvoice(int orderId)
        {
            var order = _orderService.FindOrder(orderId);

            if (order == null)
                return ConvertOrderResult.NotFound;

            if (order.IsOrderArchived || order.Status != OrderStatus.Completed)
                return ConvertOrderResult.InvalidData;

            var invoice = _orderToInvoiceMapper.Map(order);

            var result = _invoiceService.AddInvoice(invoice);

            if (!result.IsSuccess)
                return ConvertOrderResult.InvalidData;

            _orderService.ChangeOrderStatus(
                orderId,
                OrderStatus.ConvertedToInvoice);

            return ConvertOrderResult.Success;
        }
    }
}