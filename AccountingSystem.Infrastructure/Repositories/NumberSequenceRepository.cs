using AccountingSystem.Application.Repositories;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using AccountingSystem.Infrastructure.Data;
using System.Linq;

namespace AccountingSystem.Infrastructure.Repositories
{
    public class NumberSequenceRepository : INumberSequenceRepository
    {
        private readonly AppDbContext _context;

        public NumberSequenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public NumberSequence? Get(DocumentType type, int year)
        {
            return _context.NumberSequences
                .FirstOrDefault(x =>
                    x.DocumentType == type &&
                    x.Year == year);
        }

        public void Add(NumberSequence sequence)
        {
            _context.NumberSequences.Add(sequence);
        }

        public void Update(NumberSequence sequence)
        {
            _context.NumberSequences.Update(sequence);
        }
    }
}