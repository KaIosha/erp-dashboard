using backend.dtos;

namespace backend.Services.Interfaces
{
    public interface IAuthService
    {
        //register
        Task<RegisterResponseDto> RegisterUserAsync(UserRegisterDto dto);
        //login 
        Task<LoginResponseDto> LoginAsync(UserLoginDto dto);
        //refresh
        Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
        //logout
        Task<LogOutDto> LogOut(string RefreshToken);
    }
}
