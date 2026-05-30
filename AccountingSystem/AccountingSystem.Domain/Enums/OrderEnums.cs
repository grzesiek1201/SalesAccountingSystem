using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingSystem.AccountingSystem.Domain.Enums
{

    public enum OrderAddResult
    {
        Success,
        InvalidData
    }

    public enum OrderEditResult
    {
        Success,
        NotFound,
        InvalidData,
        OrderArchived
    }

    public enum ArchiveOrderResult
    {
        Success,
        NotFound,
    }

    public enum ValidateOrderResult
    {
        IsValid,
        NotValid
    }
}
