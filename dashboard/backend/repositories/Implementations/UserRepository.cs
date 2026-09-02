using backend.Data;
using backend.models;
using backend.repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.repositories.Implementations
{
    public class UserRepository : GenericRepository<Users>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> AddUserAsync(Users user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Users?> FindEmailAsync(string email)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(e => e.Email == email);
            if (user != null) return user;
            return null;
        }

        public async Task<(List<Users> Data, int TotalCount)> GetPageAsync(int page, int pageSize)
        {
            var query = _context.Users.AsNoTracking();
            var totalCount = await query.CountAsync();
            var data = await query
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, totalCount);
        }
    }
}