using backend.Data;
using backend.models;
using backend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.repositories.Implementations
{
    public class CustomerRepository : GenericRepository<Customers>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByEmailOrPhoneAsync(string email, string phone)
        {
            return await _context.Customers.AnyAsync(c => c.Email == email || c.Phone == phone);
        }

        public async Task<Customers?> GetActiveByIdAsync(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<(List<Customers> Data, int TotalCount)> GetPageAsync(int page, int pageSize)
        {
            var query = _context.Customers.AsNoTracking().Where(c => !c.IsDeleted);
            var totalCount = await query.CountAsync();
            var data = await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, totalCount);
        }
    }
}