using backend.dtos;
using backend.models;

namespace backend.Services.Interfaces
{
    public interface IJwtService
    {
        Task<JwtDto> GetJwtAsync(Users user);
    }
}
