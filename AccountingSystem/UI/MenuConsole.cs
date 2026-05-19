using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace AccountingSystem.UI
{
    internal class MenuConsole
    {
        public void MainMenu()
        {
            bool isActiveMainMenu = true;

            while (isActiveMainMenu)
            {
                Console.WriteLine("1- Go to Customers Menu 2- Go to Product Menu, 3- Go to Invoice Menu, 4- Go to Order Menu, 5- Go to Quantation Menu, 6- Go to Payment menu,\n s - to save, q -  to save and quit");
                Console.Write("Type your option: ");
                string inputChoiceMenu = Console.ReadLine();
                switch (inputChoiceMenu.Trim().ToLower())
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
                        QuantationMenu();
                        break;

                    case "6":
                        PaymentMenu();
                        break;

                    case "s":

                        break;

                    case "q":
                        Console.WriteLine("Saving data and quitting...");
                        isActiveMainMenu = false;
                        break;

                    default:
                        Console.WriteLine("Invalid input. Try again");
                        break;
                }
            }
        }

        public void CustomerMenu()
        {
            bool isCustomerMenuActive = true;
            var validator = new CustomerValidator();
            var customerService = new CustomerService(validator);
            var customerUI = new CustomerUI(customerService);

            while (isCustomerMenuActive)
            {
                Console.WriteLine("CUSTOMER MENU");
                Console.WriteLine("1- to add client, 2- to edit client, 3- to show all clients name, 4- to search a client by name or ID, 5- to archive client, r - to return to main menu");
                Console.Write("Type your option: ");
                string inputChoiceMenu = Console.ReadLine();
                switch (inputChoiceMenu.Trim().ToLower())
                {
                    case "1":
                        {
                            customerUI.AddCustomerFlow();
                        }
                        break;

                    case "2":
                        {
                            customerUI.EditCustomerFlow();

                        }
                        break;

                    case "3":
                        {
                            customerUI.GetAllCustomerFlow();

                        }
                        break;

                    case "4":
                        {
                            customerUI.FindCustomerFlow();
                        }
                        break;

                    case "5":
                        {
                            customerUI.ArchiveCustomerFlow();
                        }
                        break;

                    case "r":
                        isCustomerMenuActive = false;
                        break;

                    default:
                        Console.WriteLine("Invalid input. Try again");
                        break;
                }

            }
        }

        public void ProductMenu()
        {
            bool isProductMenuActive = true;
            var validator = new ProductValidator();
            var productService = new ProductService(validator);
            var productUI = new ProductUI(productService);
            while (isProductMenuActive)
            {
                Console.WriteLine("PRODUCT MENU");
                Console.WriteLine("1- to add product, 2- to edit prodcut, 3- to archive product,4- to search product by ID, 5- to show all products, r- to return to main menu");
                Console.Write("Type your option: ");
                string inputChoiceProduct = Console.ReadLine();
                switch (inputChoiceProduct.Trim().ToLower())
                {
                    case "1":
                        productUI.AddProductFlow();
                        break;

                    case "2":
                        productUI.EditProductFlow();
                        break;

                    case "3":
                        productUI.ArchiveProductFlow();
                        break;

                    case "4":
                        productUI.FindProductFlow();
                        break;

                    case "5":
                        productUI.GetAllProductsFlow();
                        break;

                    case "r":
                        isProductMenuActive = false;
                        break;

                    default:
                        Console.WriteLine("Invalid input. Try again");
                        break;
                }
            }
            
        }
        public void InvoiceMenu()
        {

        }

        public void OrderMenu()
        {

        }

        public void QuantationMenu()
        {

        }

        public void PaymentMenu()
        {

        }
    }
}

