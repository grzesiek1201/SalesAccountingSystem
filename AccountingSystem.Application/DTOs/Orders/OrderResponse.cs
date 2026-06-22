using AccountingSystem.Application.DTOs.Customers;

namespace AccountingSystem.Application.DTOs.Orders
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }

        public CustomerResponse Customer { get; set; }

        public List<OrderItemResponse> Items { get; set; }
    }
}
