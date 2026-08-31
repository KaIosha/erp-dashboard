using backend.dtos;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _Service;
        public AuthController(IAuthService Service)
        {
            _Service = Service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> register(UserRegisterDto dto)
        {
            var result = await _Service.RegisterUserAsync(dto);
            if (result.IsSuccess is false)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        //  POST /api/auth/login        — Login and obtain JWT token
        [HttpPost("login")]
        public async Task<IActionResult> login(UserLoginDto dto)
        {
            try
            {
                var result = await _Service.LoginAsync(dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }


        //  POST /api/auth/refresh      — Refresh JWT token
        [HttpPost("refreshToken")]
        public async Task<IActionResult> refreshToken(RefreshTokenDto dto)
        { 
           var result = await _Service.RefreshTokenAsync(dto);
            if (result.IsSuccess is false)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        //  POST /api/auth/logout       — Logout
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string RefreshToken)
        {
            var result = await _Service.LogOut(RefreshToken);
            if (result.IsSuccess is false)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }



        [Authorize]
        [HttpGet("test-token")]
        public IActionResult Test()
        {
            return Ok(
                new
                {
                    message = "Hello Youssef"
                }
                );
        }
    }
}
