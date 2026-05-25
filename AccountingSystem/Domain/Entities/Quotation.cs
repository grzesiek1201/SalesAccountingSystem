using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;

namespace AccountingSystem.Domain.Entities
{
    public class Quotation
    {
        public int Id { get; set; }

        public string QuotationNumber { get; set; }

        public QuotationStatus Status { get; set; }

        public DateTime DateCreated { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public bool IsQuotationArchived { get; set; }

        public List<QuotationItem> Items { get; set; } = new();
    }

    public class QuotationItem
    {
        public int Id { get; set; }

        public int QuotationId { get; set; }
        public Quotation Quotation { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Position { get; set; }

        public int Quantity { get; set; }

        public decimal DiscountPercent { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Total =>
            Quantity * UnitPrice * (1 - DiscountPercent / 100);
    }
}