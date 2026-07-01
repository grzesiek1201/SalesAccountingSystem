using AccountingSystem.Application.DTOs.Quotations;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.API.Controllers;

[ApiController]
[Route("api/quotations")]
public class QuotationsController : ControllerBase
{
    private readonly QuotationService _quotationService;
    private readonly QuotationResponseMapper _mapper;
    private readonly ILogger<QuotationsController> _logger;

    public QuotationsController(
        QuotationService quotationService,
        QuotationResponseMapper mapper,
        ILogger<QuotationsController> logger)
    {
        _quotationService = quotationService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("GET /api/quotations");

        var quotations = _quotationService.GetAllQuotations();

        _logger.LogInformation("Found {Count} quotations", quotations.Count);

        return Ok(quotations.Select(_mapper.Map));
    }

    [HttpGet("{id}")]
    public IActionResult Find(int id)
    {
        _logger.LogInformation("GET /api/quotations/{Id}", id);

        var quotation = _quotationService.FindQuotation(id);

        if (quotation == null)
        {
            _logger.LogWarning("Quotation not found: {Id}", id);
            return NotFound();
        }

        return Ok(_mapper.Map(quotation));
    }

    [HttpPost]
    public IActionResult Create(CreateQuotationRequest request)
    {
        _logger.LogInformation("POST /api/quotations CustomerId={CustomerId}", request.CustomerId);

        var quotation = new Quotation
        {
            CustomerId = request.CustomerId,
            DateCreated = DateTime.UtcNow,
            Status = QuotationStatus.Draft,
            Items = request.Items.Select(i => new QuotationItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                DiscountPercent = i.DiscountPercent,
                Position = i.Position
            }).ToList()
        };

        var result = _quotationService.AddQuotation(request);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Quotation create failed: {@Errors}", result.Errors);
            return BadRequest(result.Errors);
        }

        _logger.LogInformation("Quotation created: {Id}", quotation.Id);

        return Ok(_mapper.Map(quotation));
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateQuotationRequest request)
    {
        _logger.LogInformation("PUT /api/quotations/{Id}", id);

        var quotation = new Quotation
        {
            Id = id,
            CustomerId = request.CustomerId,
            Items = request.Items.Select(i => new QuotationItem
            {
                Id = i.Id ?? 0,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                DiscountPercent = i.DiscountPercent
            }).ToList()
        };

        var result = _quotationService.EditQuotation(request);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Quotation update failed {Id}: {@Errors}", id, result.Errors);
            return BadRequest(result.Errors);
        }

        var updated = _quotationService.FindQuotation(id);

        _logger.LogInformation("Quotation updated: {Id}", id);

        return Ok(_mapper.Map(updated));
    }

    [HttpPatch("{id}/archive")]
    public IActionResult Archive(int id)
    {
        _logger.LogInformation("PATCH archive quotation {Id}", id);

        var result = _quotationService.ArchiveQuotation(id);

        if (result == QuotationArchiveResult.NotFound)
        {
            _logger.LogWarning("Archive failed, not found: {Id}", id);
            return NotFound();
        }

        _logger.LogInformation("Quotation archived: {Id}", id);

        return NoContent();
    }
}