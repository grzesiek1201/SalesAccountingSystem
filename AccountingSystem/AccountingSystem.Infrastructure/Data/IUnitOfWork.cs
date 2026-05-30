using System;

namespace AccountingSystem.AccountingSystem.Infrastructure.Data
{
    public interface IUnitOfWork
    {
        void Save();
    }
}