using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    public class QuotationService
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly QuotationValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuotationService> _logger;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly OrderService _orderService;


        public QuotationService(
            IQuotationRepository quotationRepository,
            QuotationValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<QuotationService> logger,
            NumberSequenceService numberSequenceService,
            OrderService orderService)
        {
            _quotationRepository = quotationRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _numberSequenceService = numberSequenceService;
            _orderService = orderService;
        }


        public ConvertQuotationResult ConvertToOrder(int id)
        {
            _logger.LogInformation(
                "Converting quotation {Id}",
                id);


            var quotation = _quotationRepository.GetById(id);


            if (quotation == null)
                return ConvertQuotationResult.NotFound;


            if (quotation.IsQuotationArchived)
                return ConvertQuotationResult.InvalidData;


            if (quotation.Status != QuotationStatus.Accepted)
                return ConvertQuotationResult.InvalidData;



            var result = _orderService.CreateFromQuotation(quotation);



            if (!result.IsSuccess)
            {
                _logger.LogWarning(
                    "Order creation failed from quotation {Id}",
                    id);

                return ConvertQuotationResult.InvalidData;
            }



            quotation.Status = QuotationStatus.ConvertedToOrder;


            _quotationRepository.Update(quotation);
            _unitOfWork.Save();



            return ConvertQuotationResult.Success;
        }


        public QuotationAddResponse AddQuotation(Quotation quotation)
        {
            quotation.QuotationNumber =
                _numberSequenceService.GetNext(DocumentType.Quotation);


            var result =
                _validator.Validate(
                    quotation,
                    _quotationRepository.GetAll());


            if (!result.IsValid)
            {
                return new QuotationAddResponse
                {
                    Result = QuotationAddResult.InvalidData,
                    Errors = result.Errors
                };
            }


            _quotationRepository.Add(quotation);
            _unitOfWork.Save();


            return new QuotationAddResponse
            {
                Result = QuotationAddResult.Success
            };
        }


        public QuotationStatusResult ChangeQuotationStatus(
            int id,
            QuotationStatus newStatus)
        {
            var quotation =
                _quotationRepository.GetById(id);


            if (quotation == null)
                return QuotationStatusResult.NotFound;


            if (quotation.IsQuotationArchived)
                return QuotationStatusResult.InvalidOperation;


            if (quotation.Status ==
                QuotationStatus.ConvertedToOrder)
                return QuotationStatusResult.InvalidOperation;



            quotation.Status = newStatus;


            _quotationRepository.Update(quotation);
            _unitOfWork.Save();


            return QuotationStatusResult.Success;
        }


        public List<Quotation> GetAllQuotations()
        {
            return _quotationRepository.GetAll();
        }


        public Quotation? FindQuotation(int id)
        {
            return _quotationRepository.GetById(id);
        }
    }
}