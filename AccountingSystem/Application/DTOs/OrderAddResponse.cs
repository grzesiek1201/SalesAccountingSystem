using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingSystem.Application.DTOs
{
    internal class OrderAddResponse
    {
        public OrderAddResult Result { get; set; }

        public List<OrderValidationError> Errors { get; set; } = new List<OrderValidationError>();

        public bool IsSuccess => Result == OrderAddResult.Success;
    }
}
