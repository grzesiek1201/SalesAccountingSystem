using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.Application.DTOs.Quotations
{
    public class QuotationAddResponse
    {
        public QuotationAddResult Result { get; set; }

        public List<QuotationValidationError> Errors { get; set; } = new List<QuotationValidationError>();

        public bool IsSuccess => Result == QuotationAddResult.Success;
    }
}
