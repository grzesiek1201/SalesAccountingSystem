using AccountingSystem.AccountingSystem.Application.Services;
using AccountingSystem.AccountingSystem.Domain.Entities;
using AccountingSystem.AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingSystem.AccountingSystem.UI
{
    internal class OrderUI
    {
        private readonly OrderService _orderService;
        private readonly QuotationService _quotationService;
        private readonly CustomerService _customerService;
        private readonly ProductService _productService;

        public OrderUI(
            OrderService orderService,
            QuotationService quotationService,
            CustomerService customerService,
            ProductService productService)
        {
            _orderService = orderService;
            _quotationService = quotationService;
            _customerService = customerService;
            _productService = productService;
        }

        public void AddOrderFlow()
        {
            var order = GetOrderInput();
            if (order == null) return;

            AddOrderItemsFlow(order);

            var response = _orderService.AddOrder(order);

            if (!response.IsSuccess)
            {
                foreach (var error in response.Errors)
                {
                    Console.WriteLine(GetOrderErrorMessage(error));
                }
                return;
            }

            Console.WriteLine("Order added successfully");
        }

        public void EditOrderFlow()
        {
            Console.WriteLine("Edit order - fill fields below.");

            var order = GetOrderInput();
            if (order == null) return;

            _orderService.EditOrder(order);
        }

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

        public void ArchiveOrderFlow()
        {
            int id = GetOrderId();

            var result = _orderService.ArchiveOrder(id);

            switch (result)
            {
                case ArchiveOrderResult.NotFound:
                    Console.WriteLine("Order not found");
                    break;

                case ArchiveOrderResult.Success:
                    Console.WriteLine("Order archived");
                    break;
            }
        }

        // ================= INPUT =================

        private Order? GetOrderInput()
        {
            int customerId = GetInt("Customer ID: ");

            var customer = _customerService.FindCustomer(customerId);

            if (customer == null)
            {
                Console.WriteLine("Customer not found");
                return null;
            }

            return new Order
            {
                Customer = customer,
                DateCreated = DateTime.Now,
                Status = OrderStatus.Draft,
                Items = new List<OrderItem>()
            };
        }

        private void AddOrderItemsFlow(Order order)
        {
            Console.WriteLine("Add product ID (0 = finish)");

            while (true)
            {
                int productId = GetInt("Product ID: ");

                if (productId == 0)
                    break;

                var product = _productService.FindProduct(productId);

                if (product == null)
                {
                    Console.WriteLine("Product not found");
                    continue;
                }

                int qty = GetInt("Quantity: ");
                decimal discount = GetDecimal("Discount %: ");

                var unitPrice = product.Price * (1 - discount / 100m);

                order.Items.Add(new OrderItem
                {
                    Position = order.Items.Count + 1,
                    Product = product,
                    Quantity = qty,
                    DiscountPercent = discount,
                    BaseUnitPrice = unitPrice
                });

                Console.WriteLine("Item added");
            }
        }

        // ================= HELPERS =================

        private int GetOrderId()
        {
            return GetInt("Order ID: ");
        }

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

        private void PrintOrder(Order o)
        {
            Console.WriteLine($"Id: {o.Id}");
            Console.WriteLine($"Customer: {o.Customer?.Name}");
            Console.WriteLine($"Date: {o.DateCreated}");
            Console.WriteLine($"Status: {o.Status}");

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
