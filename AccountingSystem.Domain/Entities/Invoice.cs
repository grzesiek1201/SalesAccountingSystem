using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;

namespace AccountingSystem.Domain.Entities
{
    public class Invoice
    {
        public int Id { get; set; }

        public string InvoiceNumber { get; set; }

        public int OrderId { get; set; }

        public InvoiceStatus Status { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? PaidDate { get; set; }

        public decimal TotalAmount { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; }
        // SNAPSHOT CUSTOMER
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerStreet { get; set; }
        public string CustomerZipCode { get; set; }
        public string CustomerCity { get; set; }

        public bool IsInvoiceArchived { get; set; }

        public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    public class InvoiceItem
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public Invoice Invoice { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int Position { get; set; }

        public int Quantity { get; set; }

        public decimal DiscountPercent { get; set; }

        // SNAPSHOT
        public decimal BaseUnitPrice { get; set; }

        public decimal Total { get; set; }
    }
}