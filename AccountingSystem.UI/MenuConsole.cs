namespace AccountingSystem.UI
{
    public class MenuConsole
    {
        private readonly CustomerUI _customerUI;
        private readonly ProductUI _productUI;
        private readonly QuotationUI _quotationUI;
        private readonly OrderUI _orderUI;
        private readonly InvoiceUI _invoiceUI;
        private readonly PaymentUI _paymentUI;

        public MenuConsole(
            CustomerUI customerUI,
            ProductUI productUI,
            QuotationUI quotationUI,
            OrderUI orderUI,
            InvoiceUI invoiceUI,
            PaymentUI paymentUI )
        {
            _customerUI = customerUI;
            _productUI = productUI;
            _quotationUI = quotationUI;
            _orderUI = orderUI;
            _invoiceUI = invoiceUI;
            _paymentUI = paymentUI;
        }

        public void MainMenu()
        {
            bool isActive = true;

            while (isActive)
            {
                Console.WriteLine("MAIN MENU");
                Console.WriteLine("1- Customers, 2- Products, 3- Invoice, 4- Order, 5- Quotation, 6- Payment, q - quit");
                Console.Write("Type option: ");

                string input = Console.ReadLine()?.Trim().ToLower() ?? "";

                switch (input)
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
                        isActive = false;
                        break;

                    default:
                        Console.WriteLine("Invalid input");
                        break;
                }
            }
        }

        private void RunMenu(
            string title,
            string options,
            Dictionary<string, Action> actions)
        {
            bool isActive = true;

            while (isActive)
            {
                Console.WriteLine(title);
                Console.WriteLine(options);

                string input = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (input == "r")
                {
                    isActive = false;
                    continue;
                }

                if (actions.TryGetValue(input, out Action? action))
                {
                    action();
                }
                else
                {
                    Console.WriteLine("Invalid input");
                }
            }
        }

        public void CustomerMenu()
        {
            RunMenu(
                "CUSTOMER MENU",
                "1- Add, 2- Edit, 3- Archive, 4- Find, 5- List, r - back",
                new Dictionary<string, Action>
                {
                    { "1", _customerUI.AddCustomerFlow },
                    { "2", _customerUI.EditCustomerFlow },
                    { "3", _customerUI.ArchiveCustomerFlow },
                    { "4", _customerUI.FindCustomerFlow },
                    { "5", _customerUI.GetAllCustomerFlow }
                });
        }

        public void ProductMenu()
        {
            RunMenu(
                "PRODUCT MENU",
                "1- Add, 2- Edit, 3- Archive, 4- Find, 5- List, r - back",
                new Dictionary<string, Action>
                {
                    { "1", _productUI.AddProductFlow },
                    { "2", _productUI.EditProductFlow },
                    { "3", _productUI.ArchiveProductFlow },
                    { "4", _productUI.FindProductFlow },
                    { "5", _productUI.GetAllProductsFlow }
                });
        }

        public void QuotationMenu()
        {
            RunMenu(
                "QUOTATION MENU",
                "1- Add, 2- Edit, 3- Archive, 4- Find, 5- List, 6- Change status, 7- Convert, r - back",
                new Dictionary<string, Action>
                {
                    { "1", _quotationUI.AddQuotationFlow },
                    { "2", _quotationUI.EditQuotationFlow },
                    { "3", _quotationUI.ArchiveQuotationFlow },
                    { "4", _quotationUI.FindQuotationFlow },
                    { "5", _quotationUI.GetAllQuotationsFlow },
                    { "6", _quotationUI.EditStatusFlow },
                    { "7", _quotationUI.ConvertToOrderFlow }
                });
        }

        public void OrderMenu()
        {
            RunMenu(
                "ORDER MENU",
                "1- Add, 2- Edit, 3- Archive, 4- Find, 5- List, 6- Change status, 7- Convert, r - back",
                new Dictionary<string, Action>
                {
                    { "1", _orderUI.AddOrderFlow },
                    { "2", _orderUI.EditOrderFlow },
                    { "3", _orderUI.ArchiveOrderFlow },
                    { "4", _orderUI.FindOrderFlow },
                    { "5", _orderUI.GetAllOrdersFlow },
                    { "6", _orderUI.EditStatusFlow },
                    { "7", _orderUI.ConvertToInvoiceFlow }
                });
        }

        public void InvoiceMenu()
        {
            RunMenu(
                "INVOICE MENU",
                "1- Add, 2- Edit, 3- Archive, 4- Find, 5- List, 6- Change status,  r - back",
                new Dictionary<string, Action>
                {
                    { "1", _invoiceUI.AddInvoiceFlow },
                    { "2", _invoiceUI.EditInvoiceFlow },
                    { "3", _invoiceUI.ArchiveInvoiceFlow },
                    { "4", _invoiceUI.FindInvoiceFlow },
                    { "5", _invoiceUI.GetAllInvoicesFlow },
                    { "6", _invoiceUI.EditStatusFlow }
                });
        }

        public void PaymentMenu()
        {
            RunMenu(
                "PAYMENT MENU",
                "1- Add payment to invoice, 2- View payments for invoice, r - back",
                new Dictionary<string, Action>
                {
            { "1", _paymentUI.AddPaymentFlow },
            { "2", _paymentUI.GetPaymentsForInvoiceFlow }
                });
        }
    }
}
