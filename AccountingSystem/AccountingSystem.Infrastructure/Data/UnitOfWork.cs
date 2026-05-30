using System;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.AccountingSystem.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Database error while saving changes.");
            }
        }
    }
}