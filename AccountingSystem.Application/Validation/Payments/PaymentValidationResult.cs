using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingSystem.Application.Validation.Payments
{
    public class PaymentValidationResult
    {
        public List<PaymentValidationError> Errors { get; set; } = new List<PaymentValidationError>();

        public bool IsValid => Errors.Count == 0;
    }
}
