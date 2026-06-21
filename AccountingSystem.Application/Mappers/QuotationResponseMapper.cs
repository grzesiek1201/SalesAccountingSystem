using AccountingSystem.Domain.Entities;
using AccountingSystem.Application.DTOs.Customers;
using AccountingSystem.Application.DTOs.Quotations;

namespace AccountingSystem.Application.Mappers
{
    public class QuotationResponseMapper
    {
        public QuotationResponse Map(Quotation q)
        {
            return new QuotationResponse
            {
                Id = q.Id,
                QuotationNumber = q.QuotationNumber,
                Status = q.Status.ToString(),
                DateCreated = q.DateCreated,

                Customer = new CustomerResponse
                {
                    Id = q.CustomerId,
                    Email = q.CustomerEmail,
                    Name = q.CustomerName,
                    Street = q.CustomerStreet,
                    ZipCode = q.CustomerZipCode,
                    City = q.CustomerCity
                },

                Items = q.Items.Select(i => new QuotationItemResponse
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    DiscountPercent = i.DiscountPercent,
                    BaseUnitPrice = i.BaseUnitPrice,
                    Total = i.Total
                }).ToList()
            };
        }
    }
}