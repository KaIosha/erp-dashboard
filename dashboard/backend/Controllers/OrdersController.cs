using backend.dtos;
using backend.Services.Interfaces;
using backend.Validation.Order;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IValidator<CreateOrderDto> _createValidator;
        private readonly IValidator<UpdateOrderDto> _updateValidator;
        private readonly IValidator<UpdateOrderStatusDto> _statusValidator;

        public OrdersController(
            IOrderService orderService,
            IValidator<CreateOrderDto> createValidator,
            IValidator<UpdateOrderDto> updateValidator,
            IValidator<UpdateOrderStatusDto> statusValidator)
        {
            _orderService = orderService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _statusValidator = statusValidator;
        }

        [Authorize(Policy = "orders:view")]
        [HttpGet]
        public async Task<IActionResult> GetOrders(
            string? search = null,
            string? status = null,
            int? customerId = null,
            DateTime? from = null,
            DateTime? to = null,
            int page = 1,
            int pageSize = 20)
        {
            var orders = await _orderService.GetAllOrdersAsync(search, status, customerId, from, to, page, pageSize);

            return Ok(orders);
        }

        [Authorize(Policy = "orders:view")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order is null)
                return NotFound("Order not found");

            return Ok(order);
        }

        [Authorize(Policy = "orders:manage")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            CreateOrderDto dto)
        {
            var validationResult = _createValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            try
            {
                var order = await _orderService.CreateOrderAsync(dto);

                return CreatedAtAction(
                    nameof(GetOrderById),
                    new { id = order.Id },
                    order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "orders:manage")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderDto dto)
        {
            var validationResult = _updateValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            try
            {
                var order = await _orderService.UpdateOrderAsync(id, dto);

                if (order is null)
                    return NotFound("Order not found");

                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "orders:manage")]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            int id,
            UpdateOrderStatusDto dto)
        {
            var validationResult = _statusValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var updated = await _orderService.UpdateOrderStatusAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [Authorize(Policy = "orders:manage")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var deleted = await _orderService.DeleteOrderAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}