using System;
using System.Collections.Generic;
using System.Text;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Domain.Entities
{
    public class NumberSequence
    {
        public int Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public int Year { get; set; }
        public int LastNumber { get; set; }
    }
}
