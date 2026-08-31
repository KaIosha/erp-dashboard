using backend.Data;
using backend.models;
using backend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.repositories.Implementaions
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        private ApplicationDbContext _context;
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RefreshToken> FirstOrDefaultAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens.Include(u => u.User).ThenInclude(r=>r.Role).FirstOrDefaultAsync(r => r.Token == refreshToken);
            if (token is not null)
            {
                return token;
            }
            return null;
        }
    }
}
