using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Converters
{
    public class QuotationToOrderConverter
    {
        public Order Map(Quotation quotation)
        {
            ArgumentNullException.ThrowIfNull(quotation);

            return new Order
            {
                CustomerId = quotation.CustomerId,
                CustomerName = quotation.CustomerName,
                CustomerStreet = quotation.CustomerStreet,
                CustomerZipCode = quotation.CustomerZipCode,

                QuotationId = quotation.Id,

                Items = quotation.Items.Select(q => new OrderItem
                {
                    ProductId = q.ProductId,
                    ProductName = q.ProductName,
                    ProductCode = q.ProductCode,
                    VatRate = q.VatRate,
                    Unit = q.Unit,
                    Quantity = q.Quantity,
                    BaseUnitPrice = q.BaseUnitPrice,
                    DiscountPercent = q.DiscountPercent,
                    Position = q.Position,
                    Total = q.Total
                }).ToList()
            };
        }
    }
}