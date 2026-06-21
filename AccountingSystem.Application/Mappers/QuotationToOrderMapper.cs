using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System.Linq;

namespace AccountingSystem.Application.Mappers
{
    public class QuotationToOrderMapper
    {
        public Order Map(Quotation quotation)
        {
            if (quotation == null)
                return null;

            var order = new Order
            {
                CustomerId = quotation.CustomerId,
                CustomerName = quotation.CustomerName,
                CustomerStreet = quotation.CustomerStreet,
                CustomerZipCode = quotation.CustomerZipCode,
                DateCreated = DateTime.Now,
                Status = OrderStatus.Draft,
                QuotationId = quotation.Id,
                Items = quotation.Items.Select(q => new OrderItem
                {
                    ProductId = q.ProductId,
                    Quantity = q.Quantity,
                    BaseUnitPrice = q.BaseUnitPrice,
                    DiscountPercent = q.DiscountPercent,
                    Position = q.Position
                }).ToList()
            };

            return order;
        }
    }
}