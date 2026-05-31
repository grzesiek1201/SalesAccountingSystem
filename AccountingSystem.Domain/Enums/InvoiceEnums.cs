using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingSystem.Domain.Enums
{

    public enum InvoiceAddResult
    {
        Success,
        InvalidData
    }

    public enum InvoiceEditResult
    {
        Success,
        NotFound,
        InvalidData,
        InvoiceArchived
    }

    public enum ArchiveInvoiceResult
    {
        Success,
        NotFound,
    }

    public enum ValidateInvoiceResult
    {
        IsValid,
        NotValid
    }
}
