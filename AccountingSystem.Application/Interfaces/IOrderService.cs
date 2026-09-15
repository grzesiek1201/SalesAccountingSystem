using AccountingSystem.Application.DTOs.Orders;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.Interfaces
{
    public interface IOrderService
    {
        OrderAddResponse AddOrder(CreateOrderRequest request);

        OrderEditResponse EditOrder(UpdateOrderRequest request);

        List<OrderResponse> GetAllOrders();

        OrderResponse? FindOrder(int id);

        OrderAddResponse CreateOrderFromQuotation(int quotationId);


        OrderStatusResponse ConfirmOrder(int id);

        OrderStatusResponse CompleteOrder(int id);

        OrderStatusResponse CancelOrder(int id);


        ArchiveOrderResult ArchiveOrder(int id);
    }
}