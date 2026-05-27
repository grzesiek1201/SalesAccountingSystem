using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingSystem.UI
{
    internal class InvoiceUI
    {
        private readonly InvoiceService _invoiceService;
        private readonly OrderService _orderService;
        private readonly QuotationService _quotationService;
        private readonly CustomerService _customerService;
        private readonly ProductService _productService;

        public InvoiceUI(
            InvoiceService invoiceService,
            OrderService orderService,
            QuotationService quotationService,
            CustomerService customerService,
            ProductService productService)
        {
            _invoiceService = invoiceService;
            _orderService = orderService;
            _quotationService = quotationService;
            _customerService = customerService;
            _productService = productService;
        }

        public void AddInvoiceFlow()
        {
            var invoice = GetInvoiceInput();
            if (invoice == null) return;

            AddInvoiceItemsFlow(invoice);

            var response = _invoiceService.AddInvoice(invoice);

            if (!response.IsSuccess)
            {
                foreach (var error in response.Errors)
                {
                    Console.WriteLine(GetInvoiceErrorMessage(error));
                }
                return;
            }

            Console.WriteLine("Invoice added successfully");
        }

        public void EditInvoiceFlow()
        {
            Console.WriteLine("Edit invoice - fill fields below.");

            var invoice = GetInvoiceInput();
            if (invoice == null) return;

            _invoiceService.EditInvoice(invoice);
        }

        public void GetAllInvoicesFlow()
        {
            var invoices = _invoiceService.GetAllInvoices();

            foreach (var i in invoices)
            {
                PrintInvoice(i);
            }
        }

        public void FindInvoiceFlow()
        {
            int id = GetInvoiceId();

            var invoice = _invoiceService.FindInvoice(id);

            if (invoice == null)
            {
                Console.WriteLine("Invoice not found");
                return;
            }

            PrintInvoice(invoice);
        }

        public void ArchiveInvoiceFlow()
        {
            int id = GetInvoiceId();

            var result = _invoiceService.ArchiveInvoice(id);

            switch (result)
            {
                case ArchiveInvoiceResult.NotFound:
                    Console.WriteLine("Invoice not found");
                    break;

                case ArchiveInvoiceResult.Success:
                    Console.WriteLine("Invoice archived");
                    break;
            }
        }

        // ================= INPUT =================

        private Invoice? GetInvoiceInput()
        {
            int customerId = GetInt("Customer ID: ");

            var customer = _customerService.FindCustomer(customerId);

            if (customer == null)
            {
                Console.WriteLine("Customer not found");
                return null;
            }

            return new Invoice
            {
                Customer = customer,
                DateCreated = DateTime.Now,
                Status = InvoiceStatus.Draft,
                Items = new List<InvoiceItem>()
            };
        }

        private void AddInvoiceItemsFlow(Invoice invoice)
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

                invoice.Items.Add(new InvoiceItem
                {
                    Position = invoice.Items.Count + 1,
                    Product = product,
                    Quantity = qty,
                    DiscountPercent = discount,
                    BaseUnitPrice = unitPrice
                });

                Console.WriteLine("Item added");
            }
        }

        // ================= HELPERS =================

        private int GetInvoiceId()
        {
            return GetInt("Invoice ID: ");
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

        private void PrintInvoice(Invoice i)
        {
            Console.WriteLine($"Id: {i.Id}");
            Console.WriteLine($"Customer: {i.Customer?.Name}");
            Console.WriteLine($"Date: {i.DateCreated}");
            Console.WriteLine($"Status: {i.Status}");

            Console.WriteLine("--- ITEMS ---");

            decimal total = 0;

            foreach (var item in i.Items)
            {
                Console.WriteLine(
                    $"{item.Position} | {item.Product?.Name} | Qty: {item.Quantity} | Unit: {item.BaseUnitPrice} | Total: {item.Total}"
                );

                total += item.Total;
            }

            Console.WriteLine($"TOTAL: {total}");
            Console.WriteLine("--------------------------------");
        }

        private string GetInvoiceErrorMessage(InvoiceValidationError error)
        {
            return error switch
            {
                InvoiceValidationError.EmptyCustomer => "Customer is required",
                InvoiceValidationError.NoItems => "No items in invoice",
                InvoiceValidationError.InvalidQuantity => "Invalid quantity",
                InvoiceValidationError.InvalidUnitPrice => "Invalid unit price",
                InvoiceValidationError.InvalidStatus => "Invalid status",
                InvoiceValidationError.InvalidIssueDate => "Invalid issuedate",
                InvoiceValidationError.InvalidDueDate => "Invalid due date",
                InvoiceValidationError.DuplicateProduct => "Duplicate product",
                InvoiceValidationError.EmptyProduct => "Product is required",
                InvoiceValidationError.InvalidTotalAmount => "Invalid total",
                InvoiceValidationError.InvoiceNull => "Invoice is empty",
                InvoiceValidationError.InvoiceItemNull => "Invoice has no items",
                InvoiceValidationError.InvalidDiscountPercent => "Discount percent is invalid",
                InvoiceValidationError.EmptyInvoiceNumber => "Invoice number missing",
                _ => "Unknown error"
            };
        }
    }
}
