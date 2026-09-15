using AccountingSystem.Application.Converters;
using AccountingSystem.Application.DTOs.Invoices;
using AccountingSystem.Application.Factories;
using AccountingSystem.Application.Helpers;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AccountingSystem.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly InvoiceValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InvoiceService> _logger;
        private readonly InvoiceFactory _invoiceFactory;
        private readonly INumberSequenceService _numberSequenceService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly InvoiceResponseMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly OrderToInvoiceConverter _orderToInvoiceMapper;
        private readonly IInvoiceStatusCalculator _statusCalculator;


        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IPaymentRepository paymentRepository,
            InvoiceValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<InvoiceService> logger,
            InvoiceFactory invoiceFactory,
            INumberSequenceService numberSequenceService,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            InvoiceResponseMapper mapper,
            IOrderRepository orderRepository,
            OrderToInvoiceConverter orderToInvoiceMapper,
            IInvoiceStatusCalculator statusCalculator)
        {
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _invoiceFactory = invoiceFactory;
            _numberSequenceService = numberSequenceService;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _orderToInvoiceMapper = orderToInvoiceMapper;
            _statusCalculator = statusCalculator;
        }


        public InvoiceAddResponse AddInvoice(CreateInvoiceRequest request)
        {
            var customer = _customerRepository.GetById(request.CustomerId);

            if (customer == null)
                return new InvoiceAddResponse
                {
                    Result = InvoiceAddResult.InvalidData
                };


            var productIds = request.Items?
                .Select(x => x.ProductId)
                .ToList() ?? new List<int>();


            var products = _productRepository
                .GetByIds(productIds)
                .ToDictionary(x => x.Id);


            var invoice = _invoiceFactory.Create(
                request,
                customer,
                products);


            var validation = _validator.Validate(
                invoice,
                _invoiceRepository.GetAll());


            if (!validation.IsValid)
            {
                return new InvoiceAddResponse
                {
                    Result = InvoiceAddResult.InvalidData,
                    Errors = validation.Errors
                };
            }


            _invoiceRepository.Add(invoice);
            _unitOfWork.Save();


            _logger.LogInformation(
                "Invoice created: {Id}",
                invoice.Id);


            return new InvoiceAddResponse
            {
                Result = InvoiceAddResult.Success,
                CreatedId = invoice.Id
            };
        }


        public InvoiceAddResponse CreateInvoiceFromOrder(int orderId)
        {
            var order = _orderRepository.GetById(orderId);

            if (order == null || order.IsOrderArchived)
                return new InvoiceAddResponse
                {
                    Result = InvoiceAddResult.InvalidData
                };


            var invoice = _orderToInvoiceMapper.Map(order);


            if (invoice == null)
                return new InvoiceAddResponse
                {
                    Result = InvoiceAddResult.InvalidData
                };


            invoice.InvoiceNumber =
                _numberSequenceService.GetNext(DocumentType.Invoice);

            invoice.DateCreated = DateTime.UtcNow;
            invoice.IssueDate = DateTime.UtcNow;
            invoice.DueDate = DateTime.UtcNow.AddDays(14);


            var validation =
                _validator.Validate(
                    invoice,
                    _invoiceRepository.GetAll());


            if (!validation.IsValid)
            {
                return new InvoiceAddResponse
                {
                    Result = InvoiceAddResult.InvalidData,
                    Errors = validation.Errors
                };
            }


            try
            {
                _unitOfWork.BeginTransaction();

                _invoiceRepository.Add(invoice);

                order.ConvertToInvoice();

                _orderRepository.Update(order);

                _unitOfWork.Save();

                _unitOfWork.Commit();

                return new InvoiceAddResponse
                {
                    Result = InvoiceAddResult.Success
                };
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                _logger.LogError(
                    ex,
                    "Error while converting order {OrderId} to invoice",
                    orderId);

                return new InvoiceAddResponse
                {
                    Result = InvoiceAddResult.InvalidData
                };
            }
        }



        public InvoiceEditResponse EditInvoice(UpdateInvoiceRequest request)
        {
            var existing = _invoiceRepository.GetById(request.Id);


            if (existing == null)
                return new InvoiceEditResponse
                {
                    Result = InvoiceEditResult.NotFound
                };


            if (existing.IsInvoiceArchived)
                return new InvoiceEditResponse
                {
                    Result = InvoiceEditResult.InvoiceArchived
                };


            if (request.Items != null && request.Items.Any())
            {
                var items = request.Items
                    .Select(x => new InvoiceItem
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        DiscountPercent = x.DiscountPercent
                    })
                    .ToList();


                var productIds = items
                    .Select(x => x.ProductId)
                    .ToList();


                var products =
                    _productRepository
                    .GetByIds(productIds)
                    .ToDictionary(x => x.Id);


                existing.Items =
                    ItemSnapshotHelper
                    .SnapshotInvoiceItems(items, products);
            }



            if (request.CustomerId != 0 &&
                request.CustomerId != existing.CustomerId)
            {
                var customer =
                    _customerRepository.GetById(request.CustomerId);


                if (customer == null)
                    return new InvoiceEditResponse
                    {
                        Result = InvoiceEditResult.InvalidData
                    };


                existing.ApplyCustomerSnapshot(customer);
            }



            var validation =
                _validator.Validate(
                    existing,
                    _invoiceRepository
                    .GetAll()
                    .Where(x => x.Id != existing.Id)
                    .ToList(),
                    isEdit: true);



            if (!validation.IsValid)
                return new InvoiceEditResponse
                {
                    Result = InvoiceEditResult.InvalidData
                };


            _invoiceRepository.Update(existing);
            _unitOfWork.Save();


            return new InvoiceEditResponse
            {
                Result = InvoiceEditResult.Success
            };
        }



        public InvoiceStatusResponse IssueInvoice(int id)
        {
            var invoice = _invoiceRepository.GetById(id);


            if (invoice == null)
                return new InvoiceStatusResponse
                {
                    Result = InvoiceOperationResult.NotFound
                };


            try
            {
                invoice.Issue();
            }
            catch (InvalidOperationException)
            {
                return new InvoiceStatusResponse
                {
                    Result = InvoiceOperationResult.InvalidOperation
                };
            }


            _invoiceRepository.Update(invoice);
            _unitOfWork.Save();


            return new InvoiceStatusResponse
            {
                Result = InvoiceOperationResult.Success
            };
        }



        public InvoiceStatusResponse CancelInvoice(int id)
        {
            var invoice = _invoiceRepository.GetById(id);


            if (invoice == null)
                return new InvoiceStatusResponse
                {
                    Result = InvoiceOperationResult.NotFound
                };


            var payments =
                _paymentRepository
                .GetByInvoiceId(id);


            if (payments.Any(x => x.Status == PaymentStatus.Paid))
            {
                return new InvoiceStatusResponse
                {
                    Result = InvoiceOperationResult.InvalidOperation
                };
            }


            try
            {
                invoice.Cancel();
            }
            catch (InvalidOperationException)
            {
                return new InvoiceStatusResponse
                {
                    Result = InvoiceOperationResult.InvalidOperation
                };
            }


            _invoiceRepository.Update(invoice);
            _unitOfWork.Save();


            return new InvoiceStatusResponse
            {
                Result = InvoiceOperationResult.Success
            };
        }



        public ArchiveInvoiceResult ArchiveInvoice(int id)
        {
            var invoice = _invoiceRepository.GetById(id);


            if (invoice == null)
                return ArchiveInvoiceResult.NotFound;


            try
            {
                invoice.Archive();
            }
            catch (InvalidOperationException)
            {
                return ArchiveInvoiceResult.InvalidOperation;
            }


            _invoiceRepository.Update(invoice);
            _unitOfWork.Save();


            return ArchiveInvoiceResult.Success;
        }



        public List<InvoiceResponse> GetAllInvoices()
            => _invoiceRepository
                .GetAll()
                .Select(_mapper.Map)
                .ToList();



        public InvoiceResponse? FindInvoice(int id)
        {
            var invoice = _invoiceRepository.GetById(id);

            return invoice == null
                ? null
                : _mapper.Map(invoice);
        }
    }
}