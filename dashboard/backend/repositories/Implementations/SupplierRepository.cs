using backend.Data;
using backend.models;
using backend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.repositories.Implementations
{
    public class SupplierRepository : GenericRepository<Suppliers>, ISupplierRepository
    {
        public SupplierRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Suppliers.AnyAsync(s => s.Email == email);
        }

        public async Task<(List<Suppliers> Data, int TotalCount)> GetPageAsync(string? search, int page, int pageSize)
        {
            var query = _context.Suppliers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(s =>
                    s.CompanyName.ToLower().Contains(search) ||
                    s.ContactName.ToLower().Contains(search) ||
                    s.Email.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();
            var data = await query
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, totalCount);
        }
    }
}