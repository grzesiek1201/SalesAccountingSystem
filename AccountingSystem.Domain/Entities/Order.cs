using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingSystem.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; }

        public int QuotationId { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime DateCreated { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public bool IsOrderArchived { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Position { get; set; }

        public int Quantity { get; set; }

        public decimal DiscountPercent { get; set; }

        // snapshot price
        public decimal BaseUnitPrice { get; set; }

        public decimal Total =>
            Quantity * BaseUnitPrice * (1 - DiscountPercent / 100m);
    }
}

