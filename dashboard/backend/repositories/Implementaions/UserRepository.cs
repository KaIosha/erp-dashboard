using backend.Data;
using backend.models;
using backend.repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.repositories.Implementaions
{
    public class UserRepository : GenericRepository<Users>, IUserRepository
    {
        private ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
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

        public async Task<Users> FindEmailAsync(string email)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(e => e.Email == email);
            if (user != null) return user;
            return null;
        }
    }
}
