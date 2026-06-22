using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Helpers.Snapshots
{
    public static class ItemSnapshotHelper
    {
        public static List<QuotationItem> SnapshotQuotationItems(
        IEnumerable<QuotationItem> items,
        IDictionary<int, Product> products)
        {
            return items.Select(i =>
            {
                if (!products.TryGetValue(i.ProductId, out var product) || product == null)
                    throw new InvalidOperationException($"Product not found: {i.ProductId}");

                if (string.IsNullOrWhiteSpace(product.Name))
                    throw new InvalidOperationException($"Product name missing: {i.ProductId}");

                return new QuotationItem
                {
                    ProductId = i.ProductId,
                    ProductName = product.Name,
                    Quantity = i.Quantity,
                    DiscountPercent = i.DiscountPercent,
                    Position = i.Position,
                    BaseUnitPrice = product.Price,

                    Total = Math.Round(
                    i.Quantity * product.Price * (1 - i.DiscountPercent / 100m),
                    2,
                    MidpointRounding.AwayFromZero)
                };
            }).ToList();
        }

        public static List<OrderItem> SnapshotOrderItems(
            IEnumerable<OrderItem> items,
            IDictionary<int, Product> products)
        {
            return items.Select(i =>
            {
                if (!products.TryGetValue(i.ProductId, out var product))
                    throw new InvalidOperationException($"Product not found: {i.ProductId}");

                return new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = product.Name,
                    Quantity = i.Quantity,
                    DiscountPercent = i.DiscountPercent,
                    Position = i.Position,
                    BaseUnitPrice = product.Price,

                    Total = Math.Round(
                    i.Quantity * product.Price * (1 - i.DiscountPercent / 100m),
                    2,
                    MidpointRounding.AwayFromZero)
                };
            }).ToList();
        }

        public static List<InvoiceItem> SnapshotInvoiceItems(
            IEnumerable<InvoiceItem> items,
            IDictionary<int, Product> products)
        {
            return items.Select(i =>
            {
                if (!products.TryGetValue(i.ProductId, out var product))
                    throw new InvalidOperationException($"Product not found: {i.ProductId}");

                return new InvoiceItem
                {
                    ProductId = i.ProductId,
                    ProductName = product.Name,
                    Quantity = i.Quantity,
                    DiscountPercent = i.DiscountPercent,
                    Position = i.Position,
                    BaseUnitPrice = product.Price,

                    Total = Math.Round(
                    i.Quantity * product.Price * (1 - i.DiscountPercent / 100m),
                    2,
                    MidpointRounding.AwayFromZero)
                };
            }).ToList();
        }
    }
}