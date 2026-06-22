using AccountingSystem.API.DTOs.Invoices;
using AccountingSystem.API.DTOs.Orders;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.API.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;
        private readonly InvoiceResponseMapper _mapper;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(
            InvoiceService invoiceService,
            InvoiceResponseMapper mapper,
            ILogger<InvoiceController> logger)
        {
            _invoiceService = invoiceService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.LogInformation("GET /api/invoices");

            var invoices = _invoiceService.GetAllInvoices();

            _logger.LogInformation("Found {Count} invoices", invoices.Count);

            return Ok(invoices.Select(_mapper.Map));
        }

        [HttpGet("{id}")]
        public IActionResult Find(int id)
        {
            _logger.LogInformation("GET /api/invoices/{Id}", id);

            var invoice = _invoiceService.FindInvoice(id);

            if (invoice == null)
            {
                _logger.LogWarning("Invoice not found: {Id}", id);
                return NotFound();
            }

            return Ok(_mapper.Map(invoice));
        }

        [HttpPost]
        public IActionResult Create(CreateInvoiceRequest request)
        {
            _logger.LogInformation("POST /api/invoices CustomerId={CustomerId}", request.CustomerId);

            var invoice = new Invoice
            {
                CustomerId = request.CustomerId,
                DateCreated = DateTime.UtcNow,
                Status = InvoiceStatus.Draft,
                IssueDate = request.IssueDate,
                DueDate = request.DueDate,
                Items = request.Items.Select(i => new InvoiceItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    DiscountPercent = i.DiscountPercent,
                    Position = i.Position
                }).ToList()
            };

            var result = _invoiceService.AddInvoice(invoice);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Invoice create failed: {@Errors}", result.Errors);
                return BadRequest(result.Errors);
            }

            _logger.LogInformation("Invoice created: {Id}", invoice.Id);

            return Ok(_mapper.Map(invoice));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateInvoiceRequest request)
        {
            _logger.LogInformation("PUT /api/invoices/{Id}", id);

            var invoice = new Invoice
            {
                Id = id,
                CustomerId = request.CustomerId,
                Items = request.Items.Select(i => new InvoiceItem
                {
                    Id = i.Id ?? 0,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    DiscountPercent = i.DiscountPercent
                }).ToList()
            };

            var result = _invoiceService.EditInvoice(invoice);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Invoice update failed {Id}: {@Errors}", id, result.Errors);
                return BadRequest(result.Errors);
            }

            var updated = _invoiceService.FindInvoice(id);

            _logger.LogInformation("Invoice updated: {Id}", id);

            return Ok(_mapper.Map(updated));
        }

        [HttpPatch("{id}/archive")]
        public IActionResult Archive(int id)
        {
            _logger.LogInformation("PATCH archive invoice {Id}", id);

            var result = _invoiceService.ArchiveInvoice(id);

            if (result == ArchiveInvoiceResult.NotFound)
            {
                _logger.LogWarning("Archive failed, not found: {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Invoice archived: {Id}", id);

            return NoContent();
        }
    }
}
