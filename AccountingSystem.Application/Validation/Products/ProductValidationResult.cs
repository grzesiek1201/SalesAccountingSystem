using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountingSystem.Application.Validation.Products
{
    public class ProductValidationResult
    {
        public List<ProductValidationError> Errors { get; set; } = new List<ProductValidationError>();
        public bool IsValid => Errors.Count == 0;

    }
}
