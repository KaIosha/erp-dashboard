using backend.dtos;
using backend.Services.Interfaces;
using backend.Validation.Customer;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private ICustomerService _service;
        private readonly IValidator<CreateCustomerDto> _createValidator;
        private readonly IValidator<UpdateCustomerDataDto> _updateValidator;
        public CustomersController(
            ICustomerService service,
            IValidator<CreateCustomerDto> createValidator,
            IValidator<UpdateCustomerDataDto> updateValidator)
        {
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [Authorize(Policy = "customers:view")]
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 20)
        {
            var result = await _service.GetAllCustomers(page, pageSize);
            return Ok(result);
        }

        [Authorize(Policy = "customers:view")]
        [HttpGet("getById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetCustomerDataByIdAsync(id);
            if (result is not null)
            {
                return Ok(result);
            }
            return NotFound("Customer Not Found");
        }

        [Authorize(Policy = "customers:manage")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateCustomer(CreateCustomerDto dto)
        {
            var validationResult = _createValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var result = await _service.CreateCustomerAsync(dto);
            if (result is not null)
            {
                return Ok(result);
            }
            return BadRequest("Failed to create customer");
        }

        [Authorize(Policy = "customers:manage")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCustomerById(int id, UpdateCustomerDataDto dto)
        {
            var validationResult = _updateValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var result = await _service.UpdateCustomerData(id, dto);
            if (result is not null)
            {
                return Ok(result);
            }
            return BadRequest("Failed to update customer");
        }
        [Authorize(Policy = "customers:manage")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCustomerById(int id)
        {
            var result = await _service.DeleteCustomer(id);
            if (result is false)
            {
                return NotFound("Customer Not Found");
            }
            return Ok("Customer deleted successfully");
        }
    }
}
