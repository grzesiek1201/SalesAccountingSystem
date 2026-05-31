using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.Application.DTOs
{
    public class ProductAddResponse
    {
        public ProductAddResult Result { get; set; }

        public List<ProductValidationError> Errors { get; set; } = new List<ProductValidationError>();

        public bool IsSuccess => Result == ProductAddResult.Success;
    }
}
