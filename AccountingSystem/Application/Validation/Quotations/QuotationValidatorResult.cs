using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.Application.Validation.Quotations
{
    internal class QuotationValidatorResult
    {
        public List<QuotationValidatorError> Errors { get; set; } = new List<QuotationValidatorError>();
        public bool IsValid => Errors.Count == 0;
    }
}
