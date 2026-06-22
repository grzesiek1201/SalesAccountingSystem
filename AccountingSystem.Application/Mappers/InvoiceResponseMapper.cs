using AccountingSystem.Application.DTOs.Customers;
using AccountingSystem.Application.DTOs.Invoices;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Mappers
{
    public class InvoiceResponseMapper
    {
        public InvoiceResponse Map(Invoice o)
        {
            return new InvoiceResponse
            {
                Id = o.Id,
                InvoiceNumber = o.InvoiceNumber,
                Status = o.Status.ToString(),
                DateCreated = o.DateCreated,
                IssueDate = o.IssueDate,
                DueDate = o.DueDate,

                Customer = new CustomerResponse
                {
                    Id = o.CustomerId,
                    Email = o.CustomerEmail,
                    Name = o.CustomerName,
                    Street = o.CustomerStreet,
                    ZipCode = o.CustomerZipCode,
                    City = o.CustomerCity
                },

                Items = o.Items.Select(o => new InvoiceItemResponse
                {
                    ProductId = o.ProductId,
                    ProductName = o.ProductName,
                    Quantity = o.Quantity,
                    DiscountPercent = o.DiscountPercent,
                    BaseUnitPrice = o.BaseUnitPrice,
                    Total = o.Total
                }).ToList()
            };
        }
    }
}
