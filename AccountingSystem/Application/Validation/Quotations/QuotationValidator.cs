using AccountingSystem.Domain.Entities;
using System.Linq;


namespace AccountingSystem.Application.Validation.Quotations
{
    internal class QuotationValidator
    {
        public QuotationValidationResult Validate(Quotation quotation, List<Quotation> quotations)
        {
            var result = new QuotationValidationResult();

            if (quotation == null)
            {
                result.Errors.Add(QuotationValidationError.QuotationNull);
                return result;
            }

            if (quotation.Customer == null)
                result.Errors.Add(QuotationValidationError.EmptyCustomer);

            if (quotation.Items == null || quotation.Items.Count == 0)
            {
                result.Errors.Add(QuotationValidationError.NoItems);
                return result;
            }

            foreach (var item in quotation.Items)
            {
                if (item == null)
                {
                    result.Errors.Add(QuotationValidationError.QuotationItemNull);
                    continue;
                }

                if (item.Product == null)
                    result.Errors.Add(QuotationValidationError.EmptyProduct);

                if (item.Quantity <= 0)
                    result.Errors.Add(QuotationValidationError.InvalidQuantity);

                if (item.UnitPrice <= 0)
                    result.Errors.Add(QuotationValidationError.InvalidUnitPrice);

                if (item.DiscountPercent == null)
                    result.Errors.Add(QuotationValidationError.DiscountEmpty);
                
                if (item.DiscountPercent <= 0)
                    result.Errors.Add(QuotationValidationError.DiscountInvalid);
            }

            var duplicatedProducts = quotation.Items
                .Where(i => i?.Product != null)
                .GroupBy(i => i.Product.Id)
                .Any(g => g.Count() > 1);

            if (duplicatedProducts)
                result.Errors.Add(QuotationValidationError.DuplicateProduct);

            if (quotation.DateCreated == default)
                result.Errors.Add(QuotationValidationError.InvalidDate);

            if (string.IsNullOrWhiteSpace(quotation.QuotationNumber))
                result.Errors.Add(QuotationValidationError.EmptyQuotationNumber);


            
    

            return result;
        }
    }
}
