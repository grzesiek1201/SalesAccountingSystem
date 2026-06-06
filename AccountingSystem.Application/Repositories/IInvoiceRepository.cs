using System.Collections.Generic;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Repositories
{
    public interface IInvoiceRepository
    {
        List<Invoice> GetAll();
        Invoice? GetById(int id);
        void Add(Invoice invoice);
        void Update(Invoice invoice);
    }
}