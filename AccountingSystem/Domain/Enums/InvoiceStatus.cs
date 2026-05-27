using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.Domain.Enums
{
    public enum InvoiceStatus
    {
        Draft,       
        Issued,       
        Paid,        
        Overdue,      
        Cancelled     
    }
}
