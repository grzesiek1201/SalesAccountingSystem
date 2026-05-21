using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.Application.Validation.Quotations
{
    internal class QuotationValidatorResult
    {
        public List<QuotationValidationError> Errors { get; set; } = new List<QuotationValidationError>();
        public bool IsValid => Errors.Count == 0;
    }
}
