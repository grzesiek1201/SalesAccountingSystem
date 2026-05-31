using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingSystem.Application.Validation.Invoices
{
    public class InvoiceValidator
    {
        public InvoiceValidationResult Validate(Invoice invoice, List<Invoice> invoices)
        {
            var result = new InvoiceValidationResult();

            if (invoice == null)
            {
                result.Errors.Add(InvoiceValidationError.InvoiceNull);
                return result;
            }

            if (invoice.Customer == null)
                result.Errors.Add(InvoiceValidationError.EmptyCustomer);

            if (invoice.Items == null || invoice.Items.Count == 0)
            {
                result.Errors.Add(InvoiceValidationError.NoItems);
                return result;
            }

            foreach (var item in invoice.Items)
            {
                if (item == null)
                {
                    result.Errors.Add(InvoiceValidationError.InvoiceItemNull);
                    continue;
                }

                if (item.Product == null)
                    result.Errors.Add(InvoiceValidationError.EmptyProduct);

                if (item.Quantity <= 0)
                    result.Errors.Add(InvoiceValidationError.InvalidQuantity);

                if (item.BaseUnitPrice <= 0)
                    result.Errors.Add(InvoiceValidationError.InvalidUnitPrice);

                if (item.DiscountPercent < 0)
                    result.Errors.Add(InvoiceValidationError.InvalidDiscountPercent);
            }

            var duplicatedProducts = invoice.Items
                .Where(i => i?.Product != null)
                .GroupBy(i => i.Product.Id)
                .Any(g => g.Count() > 1);

            if (duplicatedProducts)
                result.Errors.Add(InvoiceValidationError.DuplicateProduct);

            if (invoice.IssueDate == default)
                result.Errors.Add(InvoiceValidationError.InvalidIssueDate);

            if (invoice.DueDate == default)
                result.Errors.Add(InvoiceValidationError.InvalidDueDate);

            if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
                result.Errors.Add(InvoiceValidationError.EmptyInvoiceNumber);

            return result;
        }
    }
}
