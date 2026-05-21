using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Application.Validation.Quotations;
using System;

namespace AccountingSystem.UI
{
    internal class MenuConsole
    {
        public void MainMenu()
        {
            bool isActiveMainMenu = true;

            while (isActiveMainMenu)
            {
                Console.WriteLine("MAIN MENU");
                Console.WriteLine("1- Customers, 2- Products, 3- Invoice, 4- Order, 5- Quotation, 6- Payment, s - save, q - quit");
                Console.Write("Type option: ");

                string input = Console.ReadLine();

                switch (input.Trim().ToLower())
                {
                    case "1":
                        CustomerMenu();
                        break;

                    case "2":
                        ProductMenu();
                        break;

                    case "3":
                        InvoiceMenu();
                        break;

                    case "4":
                        OrderMenu();
                        break;

                    case "5":
                        QuotationMenu();
                        break;

                    case "6":
                        PaymentMenu();
                        break;

                    case "q":
                        Console.WriteLine("Saving and quitting...");
                        isActiveMainMenu = false;
                        break;

                    default:
                        Console.WriteLine("Invalid input");
                        break;
                }
            }
        }

        public void CustomerMenu()
        {
            var customerValidator = new CustomerValidator();
            var customerService = new CustomerService(customerValidator);
            var customerUI = new CustomerUI(customerService);

            bool active = true;

            while (active)
            {
                Console.WriteLine("CUSTOMER MENU");
                Console.WriteLine("1- Add, 2- Edit, 3- List, 4- Find, 5- Archive, r - back");
                string input = Console.ReadLine();

                switch (input.Trim().ToLower())
                {
                    case "1": customerUI.AddCustomerFlow(); break;
                    case "2": customerUI.EditCustomerFlow(); break;
                    case "3": customerUI.GetAllCustomerFlow(); break;
                    case "4": customerUI.FindCustomerFlow(); break;
                    case "5": customerUI.ArchiveCustomerFlow(); break;
                    case "r": active = false; break;
                    default: Console.WriteLine("Invalid"); break;
                }
            }
        }

        public void ProductMenu()
        {
            var productValidator = new ProductValidator();
            var productService = new ProductService(productValidator);
            var productUI = new ProductUI(productService);

            bool active = true;

            while (active)
            {
                Console.WriteLine("PRODUCT MENU");
                Console.WriteLine("1- Add, 2- Edit, 3- Archive, 4- Find, 5- List, r - back");

                string input = Console.ReadLine();

                switch (input.Trim().ToLower())
                {
                    case "1": productUI.AddProductFlow(); break;
                    case "2": productUI.EditProductFlow(); break;
                    case "3": productUI.ArchiveProductFlow(); break;
                    case "4": productUI.FindProductFlow(); break;
                    case "5": productUI.GetAllProductsFlow(); break;
                    case "r": active = false; break;
                    default: Console.WriteLine("Invalid"); break;
                }
            }
        }

        public void QuotationMenu()
        {
            var quotationValidator = new QuotationValidator();
            var customerValidator = new CustomerValidator();
            var productValidator = new ProductValidator();

            var quotationService = new QuotationService(quotationValidator);
            var customerService = new CustomerService(customerValidator);
            var productService = new ProductService(productValidator);

            var quotationUI = new QuotationUI(
                quotationService,
                customerService,
                productService
            );

            bool active = true;

            while (active)
            {
                Console.WriteLine("QUOTATION MENU");
                Console.WriteLine("1- Add, 2- Edit, 3- Archive, 4- Find, 5- List, r - back");

                string input = Console.ReadLine();

                switch (input.Trim().ToLower())
                {
                    case "1": quotationUI.AddQuotationFlow(); break;
                    case "2": quotationUI.EditQuotationFlow(); break;
                    case "3": quotationUI.ArchiveQuotationFlow(); break;
                    case "4": quotationUI.FindQuotationFlow(); break;
                    case "5": quotationUI.GetAllQuotationsFlow(); break;
                    case "r": active = false; break;
                    default: Console.WriteLine("Invalid"); break;
                }
            }
        }

        public void InvoiceMenu() { }
        public void OrderMenu() { }
        public void PaymentMenu() { }
    }
}