using backend.dtos;
using backend.Services.Interfaces;
using backend.Validation.Product;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IProductService _service;
        private readonly IValidator<CreateProductDto> _createValidator;
        private readonly IValidator<UpdateProductDataDto> _updateValidator;
        private readonly IValidator<AdjustStockServiceFuncDto> _adjustStockValidator;
        public ProductsController(
            IProductService service,
            IValidator<CreateProductDto> createValidator,
            IValidator<UpdateProductDataDto> updateValidator,
            IValidator<AdjustStockServiceFuncDto> adjustStockValidator)
        {
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _adjustStockValidator = adjustStockValidator;
        }

        [Authorize(Policy = "products:manage")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            var validationResult = _createValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            try
            {
                var result = await _service.CreateProductAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize(Policy = "products:view")]
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories() 
        {
           var result = await _service.GetProductCategories();
           return Ok(result);
        }

        [Authorize(Policy = "products:view")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? category = null,
            [FromQuery] bool lowStock = false)
        {
            var result = await _service.GetAllProducts(page, pageSize, category, lowStock);
            return Ok(result);
        }

        [Authorize(Policy = "products:view")]
        [Authorize(Policy = "products:view")]
        [HttpGet("getById")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _service.GetProductDataByIdAsync(id);
            if (result is null)
            {
                return NotFound(new { message = "Product not found." });
            }
            return Ok(result);
        }
        [Authorize(Policy = "products:manage")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _service.DeleteProduct(id);
            if (!result)
            {
                return NotFound(new { message = "Product not found." });
            }
            return Ok(new { message = "Product deleted successfully." });
        }

        [Authorize(Policy = "products:manage")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDataDto dto)
        {
            var validationResult = _updateValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            try
            {
                var result = await _service.UpdateProductData(id, dto);
                if (result is null)
                {
                    return NotFound(new { message = "Product not found." });
                }
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "products:manage")]
        [HttpPatch("adjust-stock")]
        public async Task<IActionResult> AdjustStockQuantity([FromBody] AdjustStockServiceFuncDto dto)
        {
            var validationResult = _adjustStockValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            try
            {
                var result = await _service.AdjustStockQuantity(dto);
                if (result is null)
                {
                    return NotFound(new { message = "Product not found or is deleted." });
                }
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        } 

    }
}
