using backend.dtos;
using backend.Services.Interfaces;
using backend.Validation.Role;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRolesService _service;
        private readonly IValidator<CreateRoleDto> _validator;
        public RolesController(IRolesService service, IValidator<CreateRoleDto> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 20)
        {
            var result = await _service.GetAllRoles(page, pageSize);
            return Ok(result);
        }

        [HttpGet("getById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetRoleDataByIdAsync(id);
            if (result is not null)
            {
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole(CreateRoleDto dto)
        {
            var validationResult = _validator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var result = await _service.CreateRoleAsync(dto);
            if (result is not null)
            {
                return Ok(result);
            }
            return BadRequest("Failed to create role");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateRoleById(int id, UpdateRoleDataDto dto)
        {
            var result = await _service.UpdateRoleData(id, dto);
            if (result is not null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteRoleById(int id)
        {
            var result = await _service.DeleteRole(id);
            if (result is false)
            {
                return NotFound("Role Not Found");
            }
            return Ok();
        }
    }
}
