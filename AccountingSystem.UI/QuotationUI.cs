using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;

namespace AccountingSystem.UI
{
    public class QuotationUI
    {
        private readonly QuotationService _quotationService;
        private readonly CustomerService _customerService;
        private readonly ProductService _productService;

        public QuotationUI(
            QuotationService quotationService,
            CustomerService customerService,
            ProductService productService)
        {
            _quotationService = quotationService;
            _customerService = customerService;
            _productService = productService;
        }

        public void AddQuotationFlow()
        {
            var quotation = GetQuotationInput();
            if (quotation == null) return;

            AddQuotationItemsFlow(quotation);

            var response = _quotationService.AddQuotation(quotation);

            if (!response.IsSuccess)
            {
                foreach (var error in response.Errors)
                {
                    Console.WriteLine(GetQuotationErrorMessage(error));
                }
                return;
            }

            Console.WriteLine("Quotation added successfully");
        }

        public void EditQuotationFlow()
        {
            Console.WriteLine("Edit quotation - fill fields below.");

            var quotation = GetQuotationInput();
            if (quotation == null) return;

            _quotationService.EditQuotation(quotation);
        }

        public void GetAllQuotationsFlow()
        {
            var quotations = _quotationService.GetAllQuotations();

            foreach (var q in quotations)
            {
                PrintQuotation(q);
            }
        }

        public void FindQuotationFlow()
        {
            int id = GetQuotationId();

            var quotation = _quotationService.FindQuotation(id);

            if (quotation == null)
            {
                Console.WriteLine("Quotation not found");
                return;
            }

            PrintQuotation(quotation);
        }

        public void ArchiveQuotationFlow()
        {
            int id = GetQuotationId();

            var result = _quotationService.ArchiveQuotation(id);

            switch (result)
            {
                case ArchiveQuotationResult.NotFound:
                    Console.WriteLine("Quotation not found");
                    break;

                case ArchiveQuotationResult.Success:
                    Console.WriteLine("Quotation archived");
                    break;
            }
        }

        // ================= INPUT =================

        private Quotation? GetQuotationInput()
        {
            int customerId = GetInt("Customer ID: ");

            var customer = _customerService.FindCustomer(customerId);

            if (customer == null)
            {
                Console.WriteLine("Customer not found");
                return null;
            }

            return new Quotation
            {
                Customer = customer,
                DateCreated = DateTime.Now,
                Status = QuotationStatus.Draft,
                Items = new List<QuotationItem>()
            };
        }

        private void AddQuotationItemsFlow(Quotation quotation)
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

                quotation.Items.Add(new QuotationItem
                {
                    Position = quotation.Items.Count + 1,
                    Product = product,
                    Quantity = qty,
                    DiscountPercent = discount,
                    BaseUnitPrice = unitPrice
                });

                Console.WriteLine("Item added");
            }
        }

        // ================= HELPERS =================

        private int GetQuotationId()
        {
            return GetInt("Quotation ID: ");
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

        private void PrintQuotation(Quotation q)
        {
            Console.WriteLine($"Id: {q.Id}");
            Console.WriteLine($"Customer: {q.Customer?.Name}");
            Console.WriteLine($"Date: {q.DateCreated}");
            Console.WriteLine($"Status: {q.Status}");

            Console.WriteLine("--- ITEMS ---");

            decimal total = 0;

            foreach (var item in q.Items)
            {
                Console.WriteLine(
                    $"{item.Position} | {item.Product?.Name} | Qty: {item.Quantity} | Unit: {item.BaseUnitPrice} | Total: {item.Total}"
                );

                total += item.Total;
            }

            Console.WriteLine($"TOTAL: {total}");
            Console.WriteLine("--------------------------------");
        }

        private string GetQuotationErrorMessage(QuotationValidationError error)
        {
            return error switch
            {
                QuotationValidationError.EmptyCustomer => "Customer is required",
                QuotationValidationError.NoItems => "No items in quotation",
                QuotationValidationError.InvalidQuantity => "Invalid quantity",
                QuotationValidationError.InvalidUnitPrice => "Invalid unit price",
                QuotationValidationError.InvalidStatus => "Invalid status",
                QuotationValidationError.InvalidDate => "Invalid date",
                QuotationValidationError.DuplicateProduct => "Duplicate product",
                QuotationValidationError.EmptyProduct => "Product is required",
                QuotationValidationError.InvalidTotalAmount => "Invalid total",
                QuotationValidationError.ExpiredQuotation => "Quotation expired",
                QuotationValidationError.EmptyQuotationNumber => "Quotation number missing",
                _ => "Unknown error"
            };
        }
    }
}
