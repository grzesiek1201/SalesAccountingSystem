using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Converters
{
    public class OrderToInvoiceConverter
    {
        public Invoice Map(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            var invoice = new Invoice
            {
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                CustomerStreet = order.CustomerStreet,
                CustomerZipCode = order.CustomerZipCode,


                OrderId = order.Id,

                Items = order.Items.Select(q => new InvoiceItem
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

            invoice.TotalAmount = invoice.Items.Sum(i => i.Total);

            return invoice;
        }
    }
}