using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _service;
        public UsersController(IUserService service)
        {
            _service = service;
        }

        //  GET    /api/users           — List all users
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1,int pageSize = 20)
        {
            var result = await _service.GetAllUsers(page, pageSize);
            return Ok(result);
        }
        //  GET    /api/users/{id}     - Get user by ID
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
        //    POST / api / users           — Create user
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            var result = await _service.CreateUserAsync(dto);
            if (result is not null)
            {
                return Ok(result);
            }
            return BadRequest("Failed to create user");
        }
        //    PUT    /api/users            - Update user
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserById(int id, UpdateUserDataDto dto)
        {
            var result = await _service.UpdateUserData(id, dto);
            if (result is not null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        //     DELETE /api/users/{id}      — Delete user
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
