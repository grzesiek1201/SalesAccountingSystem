using AccountingSystem.Application.Services;
using AccountingSystem.UI;

namespace AccountingSystem.UI
{
    internal class MenuConsole
    {
        private readonly CustomerUI _customerUI;
        private readonly ProductUI _productUI;
        private readonly QuotationUI _quotationUI;

        public MenuConsole(
            CustomerUI customerUI,
            ProductUI productUI,
            QuotationUI quotationUI)
        {
            _customerUI = customerUI;
            _productUI = productUI;
            _quotationUI = quotationUI;
        }

        public void MainMenu()
        {
            bool isActiveMainMenu = true;

            while (isActiveMainMenu)
            {
                Console.WriteLine("MAIN MENU");
                Console.WriteLine("1- Customers, 2- Products, 3- Invoice, 4- Order, 5- Quotation, 6- Payment, q - quit");
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
            bool active = true;

            while (active)
            {
                Console.WriteLine("CUSTOMER MENU");
                Console.WriteLine("1- Add, 2- Edit, 3- List, 4- Find, 5- Archive, r - back");

                string input = Console.ReadLine();

                switch (input.Trim().ToLower())
                {
                    case "1": _customerUI.AddCustomerFlow(); break;
                    case "2": _customerUI.EditCustomerFlow(); break;
                    case "3": _customerUI.GetAllCustomerFlow(); break;
                    case "4": _customerUI.FindCustomerFlow(); break;
                    case "5": _customerUI.ArchiveCustomerFlow(); break;
                    case "r": active = false; break;
                    default: Console.WriteLine("Invalid"); break;
                }
            }
        }

        public void ProductMenu()
        {
            bool active = true;

            while (active)
            {
                Console.WriteLine("PRODUCT MENU");
                Console.WriteLine("1- Add, 2- Edit, 3- Archive, 4- Find, 5- List, r - back");

                string input = Console.ReadLine();

                switch (input.Trim().ToLower())
                {
                    case "1": _productUI.AddProductFlow(); break;
                    case "2": _productUI.EditProductFlow(); break;
                    case "3": _productUI.ArchiveProductFlow(); break;
                    case "4": _productUI.FindProductFlow(); break;
                    case "5": _productUI.GetAllProductsFlow(); break;
                    case "r": active = false; break;
                    default: Console.WriteLine("Invalid"); break;
                }
            }
        }

        public void QuotationMenu()
        {
            bool active = true;

            while (active)
            {
                Console.WriteLine("QUOTATION MENU");
                Console.WriteLine("1- Add, 2- Edit, 3- Archive, 4- Find, 5- List, r - back");

                string input = Console.ReadLine();

                switch (input.Trim().ToLower())
                {
                    case "1": _quotationUI.AddQuotationFlow(); break;
                    case "2": _quotationUI.EditQuotationFlow(); break;
                    case "3": _quotationUI.ArchiveQuotationFlow(); break;
                    case "4": _quotationUI.FindQuotationFlow(); break;
                    case "5": _quotationUI.GetAllQuotationsFlow(); break;
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