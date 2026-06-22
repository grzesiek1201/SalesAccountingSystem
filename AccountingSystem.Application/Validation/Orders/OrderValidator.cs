using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Validation.Orders
{
    public class OrderValidator
    {
        public OrderValidationResult Validate(
            Order order, 
            List<Order> orders, 
            bool isEdit = false)
        {
            var result = new OrderValidationResult();

            if (order == null)
            {
                result.Errors.Add(OrderValidationError.OrderNull);
                return result;
            }

            if (order.CustomerId <= 0)
                result.Errors.Add(OrderValidationError.EmptyCustomer);

            if (order.DateCreated == default && !isEdit)
                result.Errors.Add(OrderValidationError.InvalidDate);

            if (!isEdit)
            {
                if (order.Items == null || order.Items.Count == 0)
                {
                    result.Errors.Add(OrderValidationError.NoItems);
                    return result;
                }

                if (string.IsNullOrWhiteSpace(order.OrderNumber))
                    result.Errors.Add(OrderValidationError.EmptyOrderNumber);
            }

            if (order.Items != null && order.Items.Any())
            {
                foreach (var item in order.Items)
                {
                    if (item == null)
                    {
                        result.Errors.Add(OrderValidationError.OrderItemNull);
                        continue;
                    }

                    if (item.ProductId <= 0)
                        result.Errors.Add(OrderValidationError.EmptyProduct); ;

                    if (item.Quantity <= 0)
                        result.Errors.Add(OrderValidationError.InvalidQuantity);

                    if (item.BaseUnitPrice <= 0)
                        result.Errors.Add(OrderValidationError.InvalidUnitPrice);

                    if (item.DiscountPercent < 0 || item.DiscountPercent > 100)
                        result.Errors.Add(OrderValidationError.DiscountInvalid);
                }

                var duplicatedProducts = order.Items
                    .Where(i => i != null)
                    .GroupBy(i => i.ProductId)
                    .Any(g => g.Count() > 1);

                if (duplicatedProducts)
                    result.Errors.Add(OrderValidationError.DuplicateProduct);
            }
                
            return result;
        }
    }
}
