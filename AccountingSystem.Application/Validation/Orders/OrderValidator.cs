using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingSystem.Application.Validation.Orders
{
    public class OrderValidator
    {
        public OrderValidationResult Validate(Order order, List<Order> orders)
        {
            var result = new OrderValidationResult();

            if (order == null)
            {
                result.Errors.Add(OrderValidationError.OrderNull);
                return result;
            }

            if (order.Customer == null)
                result.Errors.Add(OrderValidationError.EmptyCustomer);

            if (order.Items == null || order.Items.Count == 0)
            {
                result.Errors.Add(OrderValidationError.NoItems);
                return result;
            }

            foreach (var item in order.Items)
            {
                if (item == null)
                {
                    result.Errors.Add(OrderValidationError.OrderItemNull);
                    continue;
                }

                if (item.Product == null)
                    result.Errors.Add(OrderValidationError.EmptyProduct);

                if (item.Quantity <= 0)
                    result.Errors.Add(OrderValidationError.InvalidQuantity);

                if (item.BaseUnitPrice <= 0)
                    result.Errors.Add(OrderValidationError.InvalidUnitPrice);

                if (item.DiscountPercent == null)
                {
                    result.Errors.Add(OrderValidationError.DiscountEmpty);
                }
                else if (item.DiscountPercent < 0 || item.DiscountPercent > 100)
                {
                    result.Errors.Add(OrderValidationError.DiscountInvalid);
                }
            }

            var duplicatedProducts = order.Items
                .Where(i => i?.Product != null)
                .GroupBy(i => i.Product.Id)
                .Any(g => g.Count() > 1);

            if (duplicatedProducts)
                result.Errors.Add(OrderValidationError.DuplicateProduct);

            if (order.DateCreated == default)
                result.Errors.Add(OrderValidationError.InvalidDate);

            if (string.IsNullOrWhiteSpace(order.OrderNumber))
                result.Errors.Add(OrderValidationError.EmptyOrderNumber);

            return result;
        }
    }
}
