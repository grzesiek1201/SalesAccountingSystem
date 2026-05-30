using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.AccountingSystem.Domain.Enums
{
    public enum ProductAddResult
    {
        Success,
        InvalidData
    }

    public enum ProductEditResult
    {
        Success,
        NotFound,
        InvalidData,
        ProductArchived
    }

    public enum ArchiveProductResult
    {
        Success,
        NotFound,
    }

    public enum ValidateProductResult
    {
        IsValid,
        NotValid

    }

}
