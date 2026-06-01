using System;
using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Enums;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.UI
{
    public class PaymentUI
    {
        private readonly PaymentService _paymentService;
        private readonly InvoiceService _invoiceService;

        public PaymentUI(PaymentService paymentService, InvoiceService invoiceService)
        {
            _paymentService = paymentService;
            _invoiceService = invoiceService;
        }

        public void AddPaymentFlow()
        {
            Console.WriteLine("Enter Invoice Id:");
            if (!int.TryParse(Console.ReadLine(), out int invoiceId))
            {
                Console.WriteLine("Invalid invoice id");
                return;
            }

            var invoice = _invoiceService.FindInvoice(invoiceId);

            if (invoice == null)
            {
                Console.WriteLine("Invoice not found");
                return;
            }

            Console.WriteLine($"Invoice: {invoice.InvoiceNumber}");
            Console.WriteLine($"Total: {invoice.TotalAmount}");
            Console.WriteLine($"Already paid: {invoice.Payments?.Sum(p => p.Amount) ?? 0}");
            Console.WriteLine($"Remaining: {invoice.TotalAmount - (invoice.Payments?.Sum(p => p.Amount) ?? 0)}");

            Console.WriteLine("Enter payment amount:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                Console.WriteLine("Invalid amount");
                return;
            }

            Console.WriteLine("Select payment method: 1-Cash 2-Card 3-Transfer");
            var methodInput = Console.ReadLine();

            PaymentMethod method = methodInput switch
            {
                "1" => PaymentMethod.Cash,
                "2" => PaymentMethod.Card,
                "3" => PaymentMethod.BankTransfer,
                _ => PaymentMethod.Cash
            };

            var payment = new Payment
            {
                Amount = amount,
                Method = method
            };

            var result = _paymentService.AddPayment(invoiceId, payment);

            Console.WriteLine($"Result: {result}");
        }

        public void GetPaymentsForInvoiceFlow()
        {
            Console.WriteLine("Enter Invoice Id:");
            if (!int.TryParse(Console.ReadLine(), out int invoiceId))
            {
                Console.WriteLine("Invalid invoice id");
                return;
            }

            var invoice = _invoiceService.FindInvoice(invoiceId);

            if (invoice == null)
            {
                Console.WriteLine("Invoice not found");
                return;
            }

            if (invoice.Payments == null || invoice.Payments.Count == 0)
            {
                Console.WriteLine("No payments found");
                return;
            }

            Console.WriteLine($"Payments for Invoice {invoice.InvoiceNumber}:");

            foreach (var p in invoice.Payments)
            {
                Console.WriteLine($"Id: {p.Id}, Amount: {p.Amount}, Date: {p.PaymentDate}, Method: {p.Method}");
            }
        }
    }
}