using AccountingSystem.Application.Validation.Payments;
using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingSystem.Application.DTOs
{
    public class PaymentAddResponse
    {
        public PaymentAddResult Result { get; set; }

        public List<PaymentValidationError> Errors { get; set; } = new List<PaymentValidationError>();

        public bool IsSuccess => Result == PaymentAddResult.Success;
    }
}
