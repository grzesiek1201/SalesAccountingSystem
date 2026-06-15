using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.Domain.Enums
{
    public enum QuotationStatus
    {
        Draft,
        Active,
        Inactive,
        Canceled,
        Accepted,
        ConvertedToOrder,
        Expired
    }
}
