using AccountingSystem.Application.Repositories;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.Infrastructure.Repositories
{
    public class QuotationRepository : IQuotationRepository
    {
        private readonly AppDbContext _context;

        public QuotationRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Quotation> GetAll()
            => _context.Quotations.AsNoTracking().ToList();

        public Quotation? GetById(int id)
            => _context.Quotations.FirstOrDefault(x => x.Id == id);

        public void Add(Quotation quotation)
            => _context.Quotations.Add(quotation);

        public void Update(Quotation quotation)
            => _context.Quotations.Update(quotation);
    }
}