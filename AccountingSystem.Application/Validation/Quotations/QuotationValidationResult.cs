using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.Application.Validation.Quotations
{
    public class QuotationValidationResult
    {
        public List<QuotationValidationError> Errors { get; set; } = new List<QuotationValidationError>();
        public bool IsValid => Errors.Count == 0;
    }
}
