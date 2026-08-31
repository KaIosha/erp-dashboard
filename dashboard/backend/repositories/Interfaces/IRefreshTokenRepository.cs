using backend.models;

namespace backend.repositories.Interfaces
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken> FirstOrDefaultAsync(string refreshToken);
    }
}
