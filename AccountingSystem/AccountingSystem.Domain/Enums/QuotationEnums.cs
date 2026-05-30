using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.AccountingSystem.Domain.Enums
{
    public enum QuotationAddResult
    {
        Success,
        InvalidData
    }

    public enum QuotationEditResult
    {
        Success,
        NotFound,
        InvalidData,
        QuotationArchived
    }

    public enum ArchiveQuotationResult
    {
        Success,
        NotFound,
    }

    public enum ValidateQuotationResult
    {
        IsValid,
        NotValid
    }

}
