using AccountingSystem.Application.DTOs.Orders;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.API.Controllers
{
    [ApiController]
    [Route("api/orders")]


    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly OrderResponseMapper _mapper;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            OrderService orderService,
            OrderResponseMapper mapper,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.LogInformation("GET /api/orders");

            var orders = _orderService.GetAllOrders();

            _logger.LogInformation("Found {Count} orders", orders.Count);

            return Ok(orders.Select(_mapper.Map));
        }

        [HttpGet("{id}")]
        public IActionResult Find(int id)
        {
            _logger.LogInformation("GET /api/orders/{Id}", id);

            var order = _orderService.FindOrder(id);

            if (order == null)
            {
                _logger.LogWarning("Order not found: {Id}", id);
                return NotFound();
            }

            return Ok(_mapper.Map(order));
        }

        [HttpPost]
        public IActionResult Create(CreateOrderRequest request)
        {
            _logger.LogInformation("POST /api/orders CustomerId={CustomerId}", request.CustomerId);

            var order = new Order
            {
                CustomerId = request.CustomerId,
                DateCreated = DateTime.UtcNow,
                Status = OrderStatus.Draft,
                Items = request.Items.Select(o => new OrderItem
                {
                    ProductId = o.ProductId,
                    Quantity = o.Quantity,
                    DiscountPercent = o.DiscountPercent,
                    Position = o.Position
                }).ToList()
            };

            var result = _orderService.AddOrder(order);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Order create failed: {@Errors}", result.Errors);
                return BadRequest(result.Errors);
            }

            _logger.LogInformation("Order created: {Id}", order.Id);

            return Ok(_mapper.Map(order));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateOrderRequest request)
        {
            _logger.LogInformation("PUT /api/orders/{Id}", id);

            var order = new Order
            {
                Id = id,
                CustomerId = request.CustomerId,
                Items = request.Items.Select(i => new OrderItem
                {
                    Id = i.Id ?? 0,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    DiscountPercent = i.DiscountPercent
                }).ToList()
            };

            var result = _orderService.EditOrder(order);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Order update failed {Id}: {@Errors}", id, result.Errors);
                return BadRequest(result.Errors);
            }

            var updated = _orderService.FindOrder(id);

            _logger.LogInformation("Order updated: {Id}", id);

            return Ok(_mapper.Map(updated));
        }

        [HttpPatch("{id}/archive")]
        public IActionResult Archive(int id)
        {
            _logger.LogInformation("PATCH archive order {Id}", id);

            var result = _orderService.ArchiveOrder(id);

            if (result == ArchiveOrderResult.NotFound)
            {
                _logger.LogWarning("Archive failed, not found: {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Order archived: {Id}", id);

            return NoContent();
        }
    }
}   
