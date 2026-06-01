using AccountingSystem.Domain.Enums;
using System;

namespace AccountingSystem.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }     

        public Invoice Invoice { get; set; }   

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; set; }
    }
}