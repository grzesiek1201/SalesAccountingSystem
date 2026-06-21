using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.Domain.Enums
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
        QuotationArchived,
        QuotationConverted
    }

    public enum QuotationArchiveResult
    {
        Success,
        NotFound,
    }

    public enum ConvertQuotationResult
    {
        Success,
        NotFound,
        InvalidData,
    }

    public enum ValidateQuotationResult
    {
        IsValid,
        NotValid
    }

    public enum QuotationStatusResult
    {
        Success,
        NotFound,
        InvalidOperation,
    }

}
