namespace AccountingSystem.Application.Validation.Customers
{
    public class CustomerValidator
    {

        public CustomerValidationResult Validate(Customer customer, List<Customer> customers)
        {
            var result = new CustomerValidationResult();


            if (string.IsNullOrWhiteSpace(customer.Name))
                result.Errors.Add(CustomerValidationError.EmptyName);

            else
            {
                if (customer.Name.Length > 64)
                    result.Errors.Add(CustomerValidationError.NameTooLong);

                if (customers.Exists(x => x.Name == customer.Name && x.Id != customer.Id))
                    result.Errors.Add(CustomerValidationError.DuplicateName);
            }

            if (string.IsNullOrWhiteSpace(customer.Email))
                result.Errors.Add(CustomerValidationError.EmptyEmail);
            else
            {
                if (customer.Email.Length > 64)
                    result.Errors.Add(CustomerValidationError.EmailTooLong);

                if (customers.Exists(x => x.Email == customer.Email && x.Id != customer.Id))
                    result.Errors.Add(CustomerValidationError.DuplicateEmail);

                if (customer.Email.Count(c => c == '@') != 1 ||
                    customer.Email.StartsWith("@") ||
                    customer.Email.EndsWith("@") ||
                    !customer.Email.Contains("."))
                {
                    result.Errors.Add(CustomerValidationError.InvalidEmail);
                }
            }
   

            if (string.IsNullOrWhiteSpace(customer.ZipCode))
                result.Errors.Add(CustomerValidationError.EmptyZipCode);

            else
            {
                if (customer.ZipCode.Length != 5)
                    result.Errors.Add(CustomerValidationError.InvalidZipCode);

                if (!customer.ZipCode.All(char.IsDigit))
                    result.Errors.Add(CustomerValidationError.NotDigitsZipCode);
            }

            if (string.IsNullOrWhiteSpace(customer.City))
                result.Errors.Add(CustomerValidationError.EmptyCity);
            else
            {
               if (customer.City.Length > 50)
                    result.Errors.Add(CustomerValidationError.CityTooLong);
            }
            if (string.IsNullOrWhiteSpace(customer.Street))
               result.Errors.Add(CustomerValidationError.EmptyStreet);
            else
            {
               if (customer.Street.Length > 50)
                   result.Errors.Add(CustomerValidationError.StreetTooLong);
            }
            return result;
        }
    }
}
