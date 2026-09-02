using backend.dtos;
using backend.Services.Interfaces;
using backend.Validation.Auth;
using FluentValidation;
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
        private readonly IValidator<UserRegisterDto> _registerValidator;
        private readonly IValidator<UserLoginDto> _loginValidator;
        private readonly IValidator<RefreshTokenDto> _refreshTokenValidator;
        public AuthController(
            IAuthService Service,
            IValidator<UserRegisterDto> registerValidator,
            IValidator<UserLoginDto> loginValidator,
            IValidator<RefreshTokenDto> refreshTokenValidator)
        {
            _Service = Service;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _refreshTokenValidator = refreshTokenValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> register(UserRegisterDto dto)
        {
            var validationResult = _registerValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var result = await _Service.RegisterUserAsync(dto);
            if (result.IsSuccess is false)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> login(UserLoginDto dto)
        {
            var validationResult = _loginValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

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


        [HttpPost("refreshToken")]
        public async Task<IActionResult> refreshToken(RefreshTokenDto dto)
        {
            var validationResult = _refreshTokenValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var result = await _Service.RefreshTokenAsync(dto);
            if (result.IsSuccess is false)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

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

    }
}
