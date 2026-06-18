using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.UI
{
    public class OrderUI
    {
        private readonly OrderService _orderService;
        private readonly DocumentConversionService _documentConversionService;

        public OrderUI(
            OrderService orderService,
            DocumentConversionService documentConversionService)
        {
            _orderService = orderService;
            _documentConversionService = documentConversionService;
        }

        // ================= ADD =================

        public void AddOrderFlow()
        {
            int customerId = GetCustomerIdInput();
            var items = GetOrderItemsInput();

            var order = new Order
            {
                CustomerId = customerId,
                DateCreated = DateTime.Now,
                Status = OrderStatus.Draft,
                Items = items
            };

            var response = _orderService.AddOrder(order);

            if (!response.IsSuccess)
            {
                foreach (var error in response.Errors)
                    Console.WriteLine(GetOrderErrorMessage(error));

                return;
            }

            Console.WriteLine("Order added successfully");
        }

        // ================= EDIT =================

        public void EditOrderFlow()
        {
            Console.WriteLine("Edit order");

            int orderId = GetOrderId();
            var items = GetOrderItemsInput();

            var order = new Order
            {
                Id = orderId,
                Items = items.Any() ? items : null
            };

            var result = _orderService.EditOrder(order);

            Console.WriteLine($"Result: {result}");
        }

        public void EditStatusFlow()
        {
            Console.WriteLine("Edit order status");

            int orderId = GetOrderId();

            var status = GetStatusInput();

            var order = new Order
            {
                Id = orderId,
                Status = status
            };

            var result = _orderService.ChangeOrderStatus(orderId, status);

            Console.WriteLine($"Result: {result}");
        }

        // ================= READ =================

        public void GetAllOrdersFlow()
        {
            var orders = _orderService.GetAllOrders();

            foreach (var o in orders)
            {
                PrintOrder(o);
            }
        }

        public void FindOrderFlow()
        {
            int id = GetOrderId();

            var order = _orderService.FindOrder(id);

            if (order == null)
            {
                Console.WriteLine("Order not found");
                return;
            }

            PrintOrder(order);
        }

        // ================= ARCHIVE =================

        public void ArchiveOrderFlow()
        {
            int id = GetOrderId();

            var result = _orderService.ArchiveOrder(id);

            Console.WriteLine($"Archive result: {result}");
        }

        // ================= CONVERT =================

        public void ConvertToInvoiceFlow()
        {
            int id = GetOrderId();

            var result = _documentConversionService.ConvertOrderToInvoice(id);

            switch (result)
            {
                case ConvertOrderResult.Success:
                    Console.WriteLine("Converted successfully");
                    break;

                case ConvertOrderResult.NotFound:
                    Console.WriteLine("Order not found");
                    break;

                case ConvertOrderResult.InvalidData:
                    Console.WriteLine("Order cannot be converted");
                    break;
            }
        }

        // ================= INPUT =================

        private OrderStatus GetStatusInput()
        {
            Console.WriteLine("Select status:");
            Console.WriteLine("1 - Draft");
            Console.WriteLine("2 - Confirmed");
            Console.WriteLine("3 - InProgress");
            Console.WriteLine("4 - Completed");
            Console.WriteLine("5 - Canceled");

            int value = GetInt("Option: ");

            return value switch
            {
                1 => OrderStatus.Draft,
                2 => OrderStatus.Confirmed,
                3 => OrderStatus.InProgress,
                4 => OrderStatus.Completed,
                5 => OrderStatus.Canceled,
                _ => OrderStatus.Draft,
            };
        }

        private int GetCustomerIdInput()
        {
            return GetInt("Customer ID: ");
        }

        private int GetOrderId()
        {
            return GetInt("Order ID: ");
        }

        private List<OrderItem> GetOrderItemsInput()
        {
            var items = new List<OrderItem>();
            int position = 1;

            Console.Write("Add items? (y/n): ");
            var start = Console.ReadLine();

            if (start?.ToLower() != "y")
                return items;

            while (true)
            {
                Console.Write("Add item? (y/n): ");
                var choice = Console.ReadLine();

                if (choice?.ToLower() != "y")
                    break;

                int productId = GetInt("Product ID: ");
                int quantity = GetInt("Quantity: ");
                decimal unitPrice = GetDecimal("Unit price: ");

                items.Add(new OrderItem
                {
                    Position = position++,
                    ProductId = productId,
                    Quantity = quantity,
                    BaseUnitPrice = unitPrice
                });
            }

            return items;
        }

        // ================= HELPERS =================

        private int GetInt(string label)
        {
            int value;
            Console.Write(label);

            while (!int.TryParse(Console.ReadLine(), out value))
            {
                Console.Write("Invalid number. Try again: ");
            }

            return value;
        }

        private decimal GetDecimal(string label)
        {
            decimal value;
            Console.Write(label);

            while (!decimal.TryParse(Console.ReadLine(), out value))
            {
                Console.Write("Invalid number. Try again: ");
            }

            return value;
        }

        // ================= PRINT =================

        private void PrintOrder(Order o)
        {
            Console.WriteLine($"Id: {o.Id}");
            Console.WriteLine($"Customer: {o.Customer?.Name}");
            Console.WriteLine($"Date: {o.DateCreated}");
            Console.WriteLine($"Status: {o.Status}");
            Console.WriteLine($"Order Number: {o.OrderNumber}");

            Console.WriteLine("--- ITEMS ---");

            decimal total = 0;

            foreach (var item in o.Items)
            {
                Console.WriteLine(
                    $"{item.Position} | {item.Product?.Name} | Qty: {item.Quantity} | Unit: {item.BaseUnitPrice} | Total: {item.Total}"
                );

                total += item.Total;
            }

            Console.WriteLine($"TOTAL: {total}");
            Console.WriteLine("--------------------------------");
        }

        // ================= ERRORS =================

        private string GetOrderErrorMessage(OrderValidationError error)
        {
            return error switch
            {
                OrderValidationError.EmptyCustomer => "Customer is required",
                OrderValidationError.NoItems => "No items in order",
                OrderValidationError.InvalidQuantity => "Invalid quantity",
                OrderValidationError.InvalidUnitPrice => "Invalid unit price",
                OrderValidationError.InvalidStatus => "Invalid status",
                OrderValidationError.InvalidDate => "Invalid date",
                OrderValidationError.DuplicateProduct => "Duplicate product",
                OrderValidationError.EmptyProduct => "Product is required",
                OrderValidationError.InvalidTotalAmount => "Invalid total",
                OrderValidationError.OrderCanceled => "Order is canceled",
                OrderValidationError.OrderDrafted => "Order is canceled",
                OrderValidationError.OrderCompleted => "Order is canceled",
                OrderValidationError.EmptyOrderNumber => "Order number missing",
                _ => "Unknown error"
            };
        }
    }
}
