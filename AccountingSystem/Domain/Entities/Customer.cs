using System;
using System.Collections.Generic;

namespace AccountingSystem.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
        public Wallet Wallet { get; set; } = new Wallet();
        public bool IsCustomerArchived { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

    }

    public class Wallet
    {
        public bool InDebt { get; set; }
        public double Debt { get; set; }
        public double Paid { get; set; }
    }
}
