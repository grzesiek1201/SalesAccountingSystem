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

        public QuotationUI(QuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        public void AddQuotationFlow()
        {
            var quotation = GetQuotationInput();
            if (quotation == null) return;

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
            Console.WriteLine("Edit quotation");

            var quotation = GetQuotationInput();
            if (quotation == null) return;

            var result = _quotationService.EditQuotation(quotation);

            Console.WriteLine($"Result: {result}");
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

            Console.WriteLine($"Archive result: {result}");
        }

        public void ConvertToOrderFlow()
        {
            int id = GetQuotationId();

            var result = _quotationService.ConvertToOrder(id);

            switch (result)
            {
                case ConvertQuotationResult.Success:
                    Console.WriteLine("Converted successfully");
                    break;

                case ConvertQuotationResult.NotFound:
                    Console.WriteLine("Quotation not found");
                    break;

                case ConvertQuotationResult.InvalidData:
                    Console.WriteLine("Quotation cannot be converted");
                    break;
            }
        }

        // ================= INPUT =================

        private Quotation GetQuotationInput()
        {
            int customerId = GetInt("Customer ID: ");

            var items = new List<QuotationItem>();
            int position = 1;

            while (true)
            {
                Console.Write("Add item? (y/n): ");
                var choice = Console.ReadLine();

                if (choice?.ToLower() != "y")
                    break;

                int productId = GetInt("Product ID: ");
                int quantity = GetInt("Quantity: ");
                decimal unitPrice = GetDecimal("Unit price: ");

                items.Add(new QuotationItem
                {
                    Position = position++,
                    ProductId = productId,
                    Quantity = quantity,
                    BaseUnitPrice = unitPrice
                });
            }

            return new Quotation
            {
                CustomerId = customerId,
                DateCreated = DateTime.Now,
                Status = QuotationStatus.Draft,
                Items = items
            };
        }

        // ================= HELPERS =================

        private void PrintQuotation(Quotation q)
        {
            Console.WriteLine($"Id: {q.Id}");
            Console.WriteLine($"CustomerId: {q.CustomerId}");
            Console.WriteLine($"Date: {q.DateCreated}");
            Console.WriteLine($"Status: {q.Status}");

            Console.WriteLine("--- ITEMS ---");

            decimal total = 0;

            foreach (var item in q.Items)
            {
                Console.WriteLine(
                    $"{item.Position} | ProductId: {item.ProductId} | Qty: {item.Quantity} | Unit: {item.BaseUnitPrice} | Total: {item.Total}"
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
                QuotationValidationError.AlreadyConverted => "Quotation already converted",
                _ => "Unknown error"
            };
        }

        private int GetQuotationId()
        {
            Console.Write("Quotation ID: ");
            return int.Parse(Console.ReadLine() ?? "0");
        }

        private int GetInt(string label)
        {
            Console.Write(label);
            return int.Parse(Console.ReadLine() ?? "0");
        }

        private decimal GetDecimal(string label)
        {
            Console.Write(label);
            return decimal.Parse(Console.ReadLine() ?? "0");
        }
    }
}