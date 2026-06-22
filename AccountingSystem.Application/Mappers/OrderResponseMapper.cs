using AccountingSystem.Application.DTOs.Customers;
using AccountingSystem.Application.DTOs.Orders;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Mappers
{
    public class OrderResponseMapper
    {
        public OrderResponse Map(Order o)
        {
            return new OrderResponse
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status.ToString(),
                DateCreated = o.DateCreated,

                Customer = new CustomerResponse
                {
                    Id = o.CustomerId,
                    Email = o.CustomerEmail,
                    Name = o.CustomerName,
                    Street = o.CustomerStreet,
                    ZipCode = o.CustomerZipCode,
                    City = o.CustomerCity
                },

                Items = o.Items.Select(o => new OrderItemResponse
                {
                    ProductId = o.ProductId,
                    ProductName = o.ProductName,
                    Quantity = o.Quantity,
                    DiscountPercent = o.DiscountPercent,
                    BaseUnitPrice = o.BaseUnitPrice,
                    Total = o.Total
                }).ToList()
            };
        }
    }
}