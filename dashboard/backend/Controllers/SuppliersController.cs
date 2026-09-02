using backend.dtos;
using backend.Services.Implementations;
using backend.Services.Interfaces;
using backend.Validation.Supplier;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private ISupplierService _supplierService;
        private readonly IValidator<CreateSupplierDto> _createValidator;
        private readonly IValidator<UpdateSupplierDto> _updateValidator;
        public SuppliersController(
            ISupplierService service,
            IValidator<CreateSupplierDto> createValidator,
            IValidator<UpdateSupplierDto> updateValidator)
        {
            _supplierService = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [Authorize(Policy = "suppliers:view")]
        [HttpGet]
        public async Task<IActionResult> GetSuppliers(string? search = null, int page = 1, int pageSize = 20)
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync(search, page, pageSize);
            return Ok(suppliers);
        }

        [Authorize(Policy = "suppliers:view")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);

            if (supplier is null)
                return NotFound();

            return Ok(supplier);
        }

        [Authorize(Policy = "suppliers:manage")]
        [HttpPost]
        public async Task<IActionResult> CreateSupplier(
            CreateSupplierDto dto)
        {
            var validationResult = _createValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var supplier = await _supplierService.CreateSupplierAsync(dto);

            return CreatedAtAction(
                nameof(GetSupplierById),
                new { id = supplier.Id },
                supplier);
        }

        [Authorize(Policy = "suppliers:manage")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSupplier(
            int id,
            UpdateSupplierDto dto)
        {
            var validationResult = _updateValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var supplier = await _supplierService.UpdateSupplierAsync(id, dto);

            if (supplier is null)
                return NotFound();

            return Ok(supplier);
        }

        [Authorize(Policy = "suppliers:manage")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var deleted = await _supplierService.DeleteSupplierAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
