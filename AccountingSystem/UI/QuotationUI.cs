using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.UI
{
    internal class QuotationUI
    {
        private QuotationService _quotationService;

        public QuotationUI(QuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        public void AddQuotationFlow()
        {
            var quotation = GetQuotationInput();

            var response = _quotationService.AddQuotation(quotation);

            if (!response.IsSuccess)
            {
                foreach (var error in response.Errors)
                {
                    Console.WriteLine(error);
                }
                return;
            }

            Console.WriteLine("Quotation added successfully");
        }

        public void EditQuotationlow()
        {
            Console.WriteLine("To edit quotation just fill fields below. If there is a quotation matching your Id, data will change");

            var quotation = GetQuotationInput();
            _quotationService.EditQuotation(quotation);
        }

        public void GetAllQuotationFlow()
        {
            List<Quotations> quotations = _quotationService.GetAllQuotations();

            foreach (var q in quotations)
            {
                Console.WriteLine(
                    $"Name: {q.Name}, Id: {q.Id}, Email: {q.Email},\n" +
                    $"Zip Code: {q.Address.ZipCode}, Street: {q.Address.Street}, City: {q.Address.City},\n" +
                    $"Wallet: {q.Wallet}"
                );
            }
        }

        public void FindQuotationlow()
        {
            int idSearch = GetQuotationId();

            var result = _quotationService.FindQuotation(idSearch);

            if (result != null)
            {
                Console.WriteLine(
                    $"Name: {result.Name}, Id: {result.Id}, Email: {result.Email},\n" +
                    $"Zip Code: {result.Address.ZipCode}, Street: {result.Address.Street}, City: {result.Address.City},\n" +
                    $"Wallet: {result.Wallet}"
                );
            }
            else
            {
                Console.WriteLine("Quotation not found. Try again");
            }
        }

        public void ArchiveQuotationlow()
        {
            int id = GetQuotationId();

            var result = _quotationService.ArchiveQuotation(id);

            if (result == Domain.Enums.ArchiveQuotationResult.NotFound)
            {
                Console.WriteLine("Quotation is not found. Try again");
            }
            else if (result == Domain.Enums.ArchiveQuotationResult.Success)
            {
                Console.WriteLine("Quotation has been archived.");
            }
        }

        public Quotation GetQuotationInput()
        {
            Console.Write("add name of the company/client: ");
            string name = Console.ReadLine();
        }

        public int GetQuotationId()
        {
            Console.Write("Type quotation ID: ");
            return Convert.ToInt32(Console.ReadLine());
        }

        private string GetQuotationErrorMessage(QuotationValidationError error)
        {
            return error switch
            {
                QuotationValidationError.InvalidEmail => "Email format is invalid",
                QuotationValidationError.InvalidZipCode => "Zip code is invalid",
                QuotationValidationError.EmptyEmail => "Email is required",
                QuotationValidationError.EmptyName => "Name is empty",
                QuotationValidationError.NameTooLong => "Name is too long",
                QuotationValidationError.DuplicateName => "Name is duplicated",
                QuotationValidationError.AddressNull => "Address is empty",
                QuotationValidationError.EmptyZipCode => "Zip code is empty",
                QuotationValidationError.NotDigitsZipCode => "Zip code must contain only digits",
                QuotationValidationError.EmptyCity => "City is empty",
                _ => "Unknown error"
            };
        }

    }
}
