using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingSystem.Application.DTOs.Invoices
{
    public class InvoiceAddResponse
    {
        public InvoiceAddResult Result { get; set; }

        public List<InvoiceValidationError> Errors { get; set; } = new List<InvoiceValidationError>();

        public bool IsSuccess => Result == InvoiceAddResult.Success;
    }
}
