using System;

namespace AccountingSystem.Infrastructure.Persistence
{
    public interface IUnitOfWork
    {
        void Save();
    }
}