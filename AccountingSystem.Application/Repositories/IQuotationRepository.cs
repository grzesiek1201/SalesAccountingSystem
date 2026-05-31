using System.Collections.Generic;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Repositories
{
    public interface IQuotationRepository
    {
        List<Quotation> GetAll();
        Quotation? GetById(int id);
        void Add(Quotation quotation);
    }
}