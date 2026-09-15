using AccountingSystem.Application.DTOs.Invoices;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.API.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(
            IInvoiceService invoiceService,
            ILogger<InvoiceController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        // ================= GET ALL =================
        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.LogInformation("GET /api/invoices");

            var invoices = _invoiceService.GetAllInvoices();

            return Ok(invoices);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public IActionResult Find(int id)
        {
            _logger.LogInformation("GET /api/invoices/{Id}", id);

            var invoice = _invoiceService.FindInvoice(id);

            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }

        // ================= CREATE =================
        [HttpPost]
        public IActionResult Create(CreateInvoiceRequest request)
        {
            _logger.LogInformation("POST /api/invoices CustomerId={CustomerId}", request.CustomerId);

            var result = _invoiceService.AddInvoice(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Invoice create failed: {@Errors}", result.Errors);
                return BadRequest(result.Errors);
            }

            return CreatedAtAction(nameof(Find), new { id = result.CreatedId }, null);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateInvoiceRequest request)
        {
            _logger.LogInformation("PUT /api/invoices/{Id}", id);

            request.Id = id;

            var result = _invoiceService.EditInvoice(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Invoice update failed {Id}: {@Errors}", id, result.Errors);
                return BadRequest(result.Errors);
            }

            var updated = _invoiceService.FindInvoice(id);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // ================= ARCHIVE =================
        [HttpPatch("{id}/archive")]
        public IActionResult Archive(int id)
        {
            _logger.LogInformation("PATCH archive invoice {Id}", id);

            var result = _invoiceService.ArchiveInvoice(id);

            if (result == ArchiveInvoiceResult.NotFound)
                return NotFound();

            return NoContent();
        }


        // ================= CREATE INVOICE FROM ORDER =================

        [HttpPost("from-order/{orderId}")]
        public IActionResult CreateFromOrder(int orderId)
        {
            _logger.LogInformation(
                "POST /api/invoices/from-order/{OrderId}",
                orderId);

            var result = _invoiceService.CreateInvoiceFromOrder(orderId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning(
                    "Invoice creation from order failed {OrderId}: {@Errors}",
                    orderId,
                    result.Errors);

                return BadRequest(result.Errors);
            }

            return Ok(result);
        }

        // ================= STATUS =================

        [HttpPost("{id}/issue")]
        public IActionResult Issue(int id)
        {
            _logger.LogInformation(
                "POST /api/invoices/{Id}/issue",
                id);

            var result = _invoiceService.IssueInvoice(id);

            if (result.Result == InvoiceOperationResult.NotFound)
                return NotFound();

            if (result.Result == InvoiceOperationResult.InvalidOperation)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            _logger.LogInformation(
                "POST /api/invoices/{Id}/cancel",
                id);

            var result = _invoiceService.CancelInvoice(id);

            if (result.Result == InvoiceOperationResult.NotFound)
                return NotFound();

            if (result.Result == InvoiceOperationResult.InvalidOperation)
                return BadRequest(result);

            return Ok(result);
        }
    }
}