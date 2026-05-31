using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingSystem.Application.Validation.Orders
{
    public class OrderValidationResult
    {
        public List<OrderValidationError> Errors { get; set; } = new List<OrderValidationError>();
        public bool IsValid => Errors.Count == 0;
    }
}
