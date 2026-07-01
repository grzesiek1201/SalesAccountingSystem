using AccountingSystem.Application.DTOs.Orders;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.Interfaces
{
    public interface IOrderService
    {
        OrderAddResponse AddOrder(CreateOrderRequest request);
        OrderEditResponse EditOrder(UpdateOrderRequest request);
        List<OrderResponse> GetAllOrders();
        OrderStatusResponse ChangeOrderStatus(int id, StatusOrderRequest request);
        ArchiveOrderResult ArchiveOrder(int id);
    }
}
