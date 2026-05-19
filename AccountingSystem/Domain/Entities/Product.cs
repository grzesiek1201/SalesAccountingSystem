using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AccountingSystem.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Category Category { get; set; } = new Category();
        public bool IsProductArchived { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
