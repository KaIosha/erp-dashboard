using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using backend.Validation.User;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _service;
        private readonly IValidator<CreateUserDto> _createValidator;
        private readonly IValidator<UpdateUserDataDto> _updateValidator;
        public UsersController(
            IUserService service,
            IValidator<CreateUserDto> createValidator,
            IValidator<UpdateUserDataDto> updateValidator)
        {
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [Authorize(Policy = "users:view")]
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1,int pageSize = 20)
        {
            var result = await _service.GetAllUsers(page, pageSize);
            return Ok(result);
        }
        [Authorize(Policy = "users:view")]
        [HttpGet("getById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetUserDataByIdAsync(id);
            if (result is not null)
            {
                return Ok(result);
            }
            return NotFound();
        }
       
        [Authorize(Policy = "users:manage")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            var validationResult = _createValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var result = await _service.CreateUserAsync(dto);
            if (result is not null)
            {
                return Ok(result);
            }
            return BadRequest("Failed to create user");
        }
       
        [Authorize(Policy = "users:manage")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserById(int id, UpdateUserDataDto dto)
        {
            var validationResult = _updateValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var result = await _service.UpdateUserData(id, dto);
            if (result is not null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
       
        [Authorize(Policy = "users:manage")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUserById(int id)
        {
            var result = await _service.DeleteUser(id);
            if (result is false)
            {
                return NotFound("User Not Found");
            }
            return Ok();
        }
    }
}
