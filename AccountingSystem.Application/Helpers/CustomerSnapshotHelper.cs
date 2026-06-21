using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Helpers.Snapshots
{
    public static class CustomerSnapshotHelper
    {
        public static void ApplyCustomerSnapshot(
            this Quotation quotation,
            Customer customer)
        {
            quotation.CustomerId = customer.Id;

            quotation.CustomerEmail = customer.Email;
            quotation.CustomerName = customer.Name;
            quotation.CustomerStreet = customer.Street;
            quotation.CustomerZipCode = customer.ZipCode;
            quotation.CustomerCity = customer.City;
        }

        public static void ApplyCustomerSnapshot(
            this Order order,
            Customer customer)
        {
            order.CustomerId = customer.Id;

            order.CustomerEmail = customer.Email;
            order.CustomerName = customer.Name;
            order.CustomerStreet = customer.Street;
            order.CustomerZipCode = customer.ZipCode;
            order.CustomerCity = customer.City;
        }

        public static void ApplyCustomerSnapshot(
            this Invoice invoice,
            Customer customer)
        {
            invoice.CustomerId = customer.Id;

            invoice.CustomerEmail = customer.Email;
            invoice.CustomerName = customer.Name;
            invoice.CustomerStreet = customer.Street;
            invoice.CustomerZipCode = customer.ZipCode;
            invoice.CustomerCity = customer.City;
        }
    }
}