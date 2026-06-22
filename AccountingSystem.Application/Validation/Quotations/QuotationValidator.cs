using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Validation.Quotations
{
    public class QuotationValidator
    {
        public QuotationValidationResult Validate(
            Quotation quotation,
            List<Quotation> quotations,
            bool isEdit = false)
        {
            var result = new QuotationValidationResult();

            if (quotation == null)
            {
                result.Errors.Add(QuotationValidationError.QuotationNull);
                return result;
            }

            if (quotation.CustomerId <= 0)
                result.Errors.Add(QuotationValidationError.EmptyCustomer);

            if (quotation.DateCreated == default && !isEdit)
                result.Errors.Add(QuotationValidationError.InvalidDate);

            if (!isEdit)
            {
                if (quotation.Items == null || quotation.Items.Count == 0)
                {
                    result.Errors.Add(QuotationValidationError.NoItems);
                    return result;
                }

                if (string.IsNullOrWhiteSpace(quotation.QuotationNumber))
                    result.Errors.Add(QuotationValidationError.EmptyQuotationNumber);
            }

            if (quotation.Items != null && quotation.Items.Any())
            {
                foreach (var item in quotation.Items)
                {
                    if (item == null)
                    {
                        result.Errors.Add(QuotationValidationError.QuotationItemNull);
                        continue;
                    }

                    if (item.ProductId <= 0)
                        result.Errors.Add(QuotationValidationError.EmptyProduct);

                    if (item.Quantity <= 0)
                        result.Errors.Add(QuotationValidationError.InvalidQuantity);

                    if (item.BaseUnitPrice <= 0)
                        result.Errors.Add(QuotationValidationError.InvalidUnitPrice);

                    if (item.DiscountPercent < 0 || item.DiscountPercent > 100)
                        result.Errors.Add(QuotationValidationError.DiscountInvalid);
                }

                var duplicatedProducts = quotation.Items
                    .Where(i => i != null)
                    .GroupBy(i => i.ProductId)
                    .Any(g => g.Count() > 1);

                if (duplicatedProducts)
                    result.Errors.Add(QuotationValidationError.DuplicateProduct);
            }

            return result;
        }
    }
}