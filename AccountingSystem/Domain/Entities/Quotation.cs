using System;
using System.Collections.Generic;

namespace AccountingSystem.Domain.Entities
{
    internal class Quotation
    {
        public int Id { get; set; }

        public string Status { get; set; }  

        public DateTime DateCreated { get; set; }

        public Customer Customer { get; set; }

        public List<QuotationItem> Items { get; set; } = new List<QuotationItem>();
    }

    internal class QuotationItem
    {
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Total => Quantity * UnitPrice;
    }
}