using System.Collections.Generic;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Repositories
{
    public interface IPaymentRepository
    {
        void Add(Payment payment);

        Payment? GetById(int id);

        List<Payment> GetAll();

        List<Payment> GetByInvoiceId(int invoiceId);

        void Delete(Payment payment);
    }
}