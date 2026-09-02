using backend.dtos;
using backend.Services.Interfaces;
using backend.Validation.PurchaseOrder;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IValidator<CreatePurchaseOrderDto> _createValidator;
        private readonly IValidator<UpdatePurchaseOrderDto> _updateValidator;
        private readonly IValidator<UpdatePurchaseOrderStatusDto> _statusValidator;

        public PurchaseOrdersController(
            IPurchaseOrderService purchaseOrderService,
            IValidator<CreatePurchaseOrderDto> createValidator,
            IValidator<UpdatePurchaseOrderDto> updateValidator,
            IValidator<UpdatePurchaseOrderStatusDto> statusValidator)
        {
            _purchaseOrderService = purchaseOrderService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _statusValidator = statusValidator;
        }

        [Authorize(Policy = "purchases:view")]
        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrders(string? search = null, int page = 1, int pageSize = 20)
        {
            var purchaseOrders = await _purchaseOrderService.GetAllPurchaseOrdersAsync(search, page, pageSize);

            return Ok(purchaseOrders);
        }

        [Authorize(Policy = "purchases:view")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPurchaseOrderById(int id)
        {
            var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);

            if (purchaseOrder is null)
                return NotFound("Purchase order not found");

            return Ok(purchaseOrder);
        }

        [Authorize(Policy = "purchases:manage")]
        [HttpPost]
        public async Task<IActionResult> CreatePurchaseOrder(
            CreatePurchaseOrderDto dto)
        {
            var validationResult = _createValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            try
            {
                var purchaseOrder =
                    await _purchaseOrderService.CreatePurchaseOrderAsync(dto);

                return CreatedAtAction(
                    nameof(GetPurchaseOrderById),
                    new { id = purchaseOrder.Id },
                    purchaseOrder);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "purchases:manage")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePurchaseOrder(int id, UpdatePurchaseOrderDto dto)
        {
            var validationResult = _updateValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var purchaseOrder =
                await _purchaseOrderService.UpdatePurchaseOrderAsync(id, dto);

            if (purchaseOrder is null)
                return NotFound("Purchase order not found");

            return Ok(purchaseOrder);
        }

        [Authorize(Policy = "purchases:manage")]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdatePurchaseOrderStatus(
            int id,
            UpdatePurchaseOrderStatusDto dto)
        {
            var validationResult = _statusValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var updated =
                await _purchaseOrderService
                    .UpdatePurchaseOrderStatusAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }
    }
}
