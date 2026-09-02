using backend.Data;
using backend.models;
using backend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.repositories.Implementations
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> FirstOrDefaultAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens.Include(u => u.User).ThenInclude(r => r.Role).FirstOrDefaultAsync(r => r.Token == refreshToken);
            if (token is not null)
            {
                return token;
            }
            return null;
        }
    }
}