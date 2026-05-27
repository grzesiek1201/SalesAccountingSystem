using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountingSystem.Application.Validation.Customers
{
    internal class CustomerValidationResult
    {
        public List<CustomerValidationError> Errors { get; set; } = new List<CustomerValidationError>();
        public bool IsValid => Errors.Count == 0;

    }
}
