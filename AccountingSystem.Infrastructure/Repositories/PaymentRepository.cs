using System.Collections.Generic;
using System.Linq;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly List<Payment> _payments = new();

        public void Add(Payment payment)
        {
            payment.Id = _payments.Count > 0 ? _payments.Max(p => p.Id) + 1 : 1;
            _payments.Add(payment);
        }

        public Payment? GetById(int id)
        {
            return _payments.FirstOrDefault(p => p.Id == id);
        }

        public List<Payment> GetAll()
        {
            return _payments;
        }

        public List<Payment> GetByInvoiceId(int invoiceId)
        {
            return _payments.Where(p => p.Invoice.Id == invoiceId).ToList();
        }

        public void Delete(Payment payment)
        {
            _payments.Remove(payment);
        }
    }
}