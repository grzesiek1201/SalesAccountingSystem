using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingSystem.AccountingSystem.Application.Validation.Invoices
{
    internal class InvoiceValidationResult
    {
        public List<InvoiceValidationError> Errors { get; set; } = new List<InvoiceValidationError>();
        public bool IsValid => Errors.Count == 0;
    }
}
