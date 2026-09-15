using AccountingSystem.Application.DTOs.Orders;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            IOrderService orderService,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // ================= GET ALL =================
        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.LogInformation("GET /api/orders");

            var orders = _orderService.GetAllOrders();

            return Ok(orders);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public IActionResult Find(int id)
        {
            _logger.LogInformation("GET /api/orders/{Id}", id);

            var order = _orderService.FindOrder(id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // ================= CREATE =================
        [HttpPost]
        public IActionResult Create(CreateOrderRequest request)
        {
            _logger.LogInformation("POST /api/orders CustomerId={CustomerId}", request.CustomerId);

            var result = _orderService.AddOrder(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Order create failed: {@Errors}", result.Errors);
                return BadRequest(result.Errors);
            }

            return CreatedAtAction(nameof(Find), new { id = result.CreatedId }, null);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateOrderRequest request)
        {
            _logger.LogInformation("PUT /api/orders/{Id}", id);

            request.Id = id;

            var result = _orderService.EditOrder(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Order update failed {Id}: {@Errors}", id, result.Errors);
                return BadRequest(result.Errors);
            }

            var updated = _orderService.FindOrder(id);

            if (updated == null)
                return NotFound();

            return Ok(result);
        }

        // ================= ARCHIVE =================
        [HttpPatch("{id}/archive")]
        public IActionResult Archive(int id)
        {
            _logger.LogInformation("PATCH archive order {Id}", id);

            var result = _orderService.ArchiveOrder(id);

            if (result == ArchiveOrderResult.NotFound)
                return NotFound();

            return NoContent();
        }

        // ================= CREATE ORDER FROM QUOTATION =================

        [HttpPost("from-quotation/{quotationId}")]
        public IActionResult CreateFromQuotation(int quotationId)
        {
            _logger.LogInformation(
                "POST /api/orders/from-quotation/{QuotationId}",
                quotationId);

            var result = _orderService.CreateOrderFromQuotation(quotationId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning(
                    "Order creation from quotation failed {QuotationId}: {@Errors}",
                    quotationId,
                    result.Errors);

                return BadRequest(result.Errors);
            }

            return Ok(result);
        }

        // ================= STATUS =================

        [HttpPost("{id}/confirm")]
        public IActionResult Confirm(int id)
        {
            _logger.LogInformation(
                "POST /api/orders/{Id}/confirm",
                id);

            var result = _orderService.ConfirmOrder(id);

            if (result.Result == OrderStatusResult.NotFound)
                return NotFound();

            if (result.Result == OrderStatusResult.InvalidOperation)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("{id}/complete")]
        public IActionResult Complete(int id)
        {
            _logger.LogInformation(
                "POST /api/orders/{Id}/complete",
                id);

            var result = _orderService.CompleteOrder(id);

            if (result.Result == OrderStatusResult.NotFound)
                return NotFound();

            if (result.Result == OrderStatusResult.InvalidOperation)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            _logger.LogInformation(
                "POST /api/orders/{Id}/cancel",
                id);

            var result = _orderService.CancelOrder(id);

            if (result.Result == OrderStatusResult.NotFound)
                return NotFound();

            if (result.Result == OrderStatusResult.InvalidOperation)
                return BadRequest(result);

            return Ok(result);
        }
    }
}