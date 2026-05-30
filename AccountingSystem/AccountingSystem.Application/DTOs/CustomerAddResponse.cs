using AccountingSystem.AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.AccountingSystem.Application.DTOs
{
    public class CustomerAddResponse
    {
        public CustomerAddResult Result { get; set; }

        public List<CustomerValidationError> Errors { get; set; } = new List<CustomerValidationError>();

        public bool IsSuccess => Result == CustomerAddResult.Success;
    }
}
