using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.UI
{
    public class InvoiceUI
    {
        private readonly InvoiceService _invoiceService;

        public InvoiceUI(
            InvoiceService invoiceService,
            OrderService orderService,
            QuotationService quotationService,
            CustomerService customerService,
            ProductService productService)
        {
            _invoiceService = invoiceService;
        }

        // ================= ADD =================

        public void AddInvoiceFlow()
        {
            int customerId = GetCustomerIdInput();
            var items = GetInvoiceItemsInput();

            var invoice = new Invoice
            {
                CustomerId = customerId,
                DateCreated = DateTime.Now,
                Status = InvoiceStatus.Draft,
                Items = items
            };

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

        // ================= EDIT =================

        public void EditInvoiceFlow()
        {
            Console.WriteLine("Edit invoice");

            int invoiceId = GetInvoiceId();
            var items = GetInvoiceItemsInput();

            var invoice = new Invoice
            {
                Id = invoiceId,
                Items = items.Any() ? items : null
            };

            var result = _invoiceService.EditInvoice(invoice);

            Console.WriteLine($"Result: {result}");
        }

        public void EditStatusFlow()
        {
            Console.WriteLine("Edit invoice status");

            int invoiceId = GetInvoiceId();

            var status = GetStatusInput();

            var invoice = new Invoice
            {
                Id = invoiceId,
                Status = status
            };

            var result = _invoiceService.ChangeInvoiceStatus(invoiceId, status);

            Console.WriteLine($"Result: {result}");
        }

        // ================= READ =================

        public void GetAllInvoicesFlow()
        {
            var invoices = _invoiceService.GetAllInvoices();

            foreach (var i in invoices)
                PrintInvoice(i);
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

        // ================= ARCHIVE =================

        public void ArchiveInvoiceFlow()
        {
            int id = GetInvoiceId();

            var result = _invoiceService.ArchiveInvoice(id);

            Console.WriteLine($"Archive result: {result}");
        }

        // ================= INPUT =================

        private InvoiceStatus GetStatusInput()
        {
            Console.WriteLine("Select status:");
            Console.WriteLine("1 - Draft");
            Console.WriteLine("2 - Issued");
            Console.WriteLine("3 - Cancelled");
            Console.WriteLine("4 - Unpaid");

            int value = GetInt("Option: ");

            return value switch
            {
                1 => InvoiceStatus.Draft,
                2 => InvoiceStatus.Issued,
                3 => InvoiceStatus.Cancelled,
                4 => InvoiceStatus.Unpaid,
                _ => InvoiceStatus.Draft
            };
        }
        private int GetCustomerIdInput()
        {
            return GetInt("Customer ID: ");
        }

        private int GetInvoiceId()
        {
            return GetInt("Invoice ID: ");
        }

        private List<InvoiceItem> GetInvoiceItemsInput()
        {
            var items = new List<InvoiceItem>();
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

                items.Add(new InvoiceItem
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

        private void PrintInvoice(Invoice i)
        {
            Console.WriteLine($"Id: {i.Id}");
            Console.WriteLine($"Customer: {i.Customer?.Name}");
            Console.WriteLine($"Date: {i.DateCreated}");
            Console.WriteLine($"Status: {i.Status}");
            Console.WriteLine($"Invoice Number: {i.InvoiceNumber}");

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

        // ================= ERRORS =================

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
                InvoiceValidationError.InvalidDateCreated => "Invalid created date",
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
