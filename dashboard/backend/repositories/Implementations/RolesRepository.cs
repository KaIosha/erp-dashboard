using backend.Data;
using backend.models;
using backend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.repositories.Implementations
{
    public class RolesRepository : GenericRepository<Roles>, IRolesRepository
    {
        public RolesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(List<Roles> Data, int TotalCount)> GetPageAsync(int page, int pageSize)
        {
            var query = _context.Roles.AsNoTracking();
            var totalCount = await query.CountAsync();
            var data = await query
                .OrderBy(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, totalCount);
        }
    }
}