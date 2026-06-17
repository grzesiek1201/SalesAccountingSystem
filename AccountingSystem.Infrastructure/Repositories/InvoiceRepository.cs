using AccountingSystem.Application.Repositories;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;

        public InvoiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Invoice> GetAll()
            => _context.Invoices
                 .Include(x => x.Items)
                 .Include(x => x.Customer)
                 .AsNoTracking()
                 .ToList();

        public Invoice? GetById(int id)
            => _context.Invoices
                .Include(x => x.Items)
                .Include(x => x.Customer)
                .FirstOrDefault(x => x.Id == id);

        public void Add(Invoice invoice)
            => _context.Invoices.Add(invoice);

        public void Update(Invoice invoice)
            => _context.Invoices.Update(invoice);
    }
}