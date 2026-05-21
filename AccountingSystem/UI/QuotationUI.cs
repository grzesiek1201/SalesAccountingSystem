using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.UI
{
    internal class QuotationUI
    {
        private QuotationService _quotationService;
        private CustomerService _customerService;
        private ProductService _productService;


        public QuotationUI(
    QuotationService quotationService,
    CustomerService customerService,
    ProductService productService
    )
        {
            
            _quotationService = quotationService;
            _customerService = customerService;
            _productService = productService; ;
        }

        public void AddQuotationFlow()
        {
            var quotation = GetQuotationInput();

            var response = _quotationService.AddQuotation(quotation);

            if (!response.IsSuccess)
            {
                foreach (var error in response.Errors)
                {
                    Console.WriteLine(error);
                }
                return;
            }

            Console.WriteLine("Quotation added successfully");
        }

        public void EditQuotationFlow()
        {
            Console.WriteLine("To edit quotation just fill fields below. If there is a quotation matching your Id, data will change");

            var quotation = GetQuotationInput();
            _quotationService.EditQuotation(quotation);
        }

        public void GetAllQuotationsFlow()
        {
            List<Quotation> quotations = _quotationService.GetAllQuotations();

            foreach (var q in quotations)
            {
                Console.WriteLine($"Id: {q.Id}");
                Console.WriteLine($"Customer: {q.Customer}");
                Console.WriteLine($"Date created: {q.DateCreated}");
                Console.WriteLine($"Status: {q.Status}");
                Console.WriteLine();

                Console.WriteLine("--- ITEMS ---");

                decimal totalSum = 0;

                foreach (var item in q.Items)
                {
                    Console.WriteLine(
                        $"{item.Position} | {item.Product?.Name} | Qty: {item.Quantity} | Unit: {item.UnitPrice} | Total: {item.Total}"
                    );

                    totalSum += item.Total;
                }

                Console.WriteLine();
                Console.WriteLine($"TOTAL: {totalSum}");
                Console.WriteLine(new string('-', 40));
                Console.WriteLine();
            }
        }

        public void FindQuotationFlow()
        {
            int idSearch = GetQuotationId();

            var result = _quotationService.FindQuotation(idSearch);

            if (result != null)
            {
                Console.WriteLine($"Id: {result.Id}");
                Console.WriteLine($"Customer: {result.Customer}");
                Console.WriteLine($"Date created: {result.DateCreated}");
                Console.WriteLine();

                Console.WriteLine("--- ITEMS ---");

                decimal totalSum = 0;

                foreach (var item in result.Items)
                {
                    Console.WriteLine(
                        $"{item.Position} | {item.Product?.Name} | Qty: {item.Quantity} | Unit: {item.UnitPrice} | Total: {item.Total}"
                    );

                    totalSum += item.Total;
                }

                Console.WriteLine();
                Console.WriteLine($"TOTAL: {totalSum}");
            }
            else
            {
                Console.WriteLine("Quotation not found. Try again");
            }
        }
        public void ArchiveQuotationFlow()
        {
            int id = GetQuotationId();

            var result = _quotationService.ArchiveQuotation(id);

            if (result == Domain.Enums.ArchiveQuotationResult.NotFound)
            {
                Console.WriteLine("Quotation is not found. Try again");
            }
            else if (result == Domain.Enums.ArchiveQuotationResult.Success)
            {
                Console.WriteLine("Quotation has been archived.");
            }
        }

        public Quotation GetQuotationInput()
        {
            Console.Write("Add customer id: ");

            int customerId = int.Parse(Console.ReadLine());

            Customer customer = _customerService.FindCustomer(customerId);

            if (customer == null)
            {
                Console.WriteLine("Customer not found.");
                return null;
            }

            Quotation quotation = new Quotation
            {
                Customer = customer,
                DateCreated = DateTime.Now,
                Status = "Active"
            };

            return quotation;
        }


        public void AddQuotationItemsFlow(Quotation quotation)
        {
            while (true)
            {
                Console.Write("Add product id (0 to finish): ");

                int productId = int.Parse(Console.ReadLine());

                if (productId == 0)
                {
                    break;
                }

                Product product = _productService.FindProduct(productId);

                if (product == null)
                {
                    Console.WriteLine("Product not found.");
                    continue;
                }

                Console.Write("Add quantity: ");
                int quantity = int.Parse(Console.ReadLine());

                Console.Write("Add discount percent: ");
                decimal discountPercent = decimal.Parse(Console.ReadLine());

                decimal unitPrice =
                    product.Price * (1 - discountPercent / 100m);

                QuotationItem item = new QuotationItem
                {
                    Position = quotation.Items.Count + 1,
                    Product = product,
                    Quantity = quantity,
                    DiscountPercent = discountPercent,
                    UnitPrice = unitPrice
                };

                quotation.Items.Add(item);

                Console.WriteLine("Item added.");
                Console.WriteLine();
            }
        }

        public int GetQuotationId()
        {
            Console.Write("Type quotation ID: ");
            return Convert.ToInt32(Console.ReadLine());
        }

        private string GetQuotationErrorMessage(QuotationValidatorError error)
        {
            return error switch
            {
                QuotationValidatorError.EmptyCustomer => "Customer is empty",
                QuotationValidatorError.NoItems => "No items in quotation",
                QuotationValidatorError.InvalidQuantity => "Quantity is invalid",
                QuotationValidatorError.InvalidUnitPrice => "Unit price is invalid",
                QuotationValidatorError.InvalidStatus => "Status is invalid",
                QuotationValidatorError.InvalidDate => "Date is invalid",
                QuotationValidatorError.DuplicateProduct => "Product is duplicated",
                QuotationValidatorError.QuotationAlreadyAccepted => "Quatation is already accepted",
                QuotationValidatorError.QuotationAlreadyRejected => "Quatation is already rejected",
                QuotationValidatorError.QuotationAlreadyConvertedToOrder => "Quatation is already converted into order",
                _ => "Unknown error"
            };
        }

    }
}
