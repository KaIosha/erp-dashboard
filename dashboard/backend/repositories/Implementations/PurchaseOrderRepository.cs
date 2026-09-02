using backend.Data;
using backend.models;
using backend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.repositories.Implementations
{
    public class PurchaseOrderRepository : GenericRepository<PurchaseOrders>, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(List<PurchaseOrders> Data, int TotalCount)> GetPageAsync(string? search, int page, int pageSize)
        {
            IQueryable<PurchaseOrders> query = _context.PurchaseOrders.AsNoTracking().Include(p => p.Lines);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(p => p.Status.ToLower().Contains(search)
                    || p.Supplier.CompanyName.ToLower().Contains(search)
                    || p.Supplier.ContactName.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();
            var data = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, totalCount);
        }

        public async Task<PurchaseOrders?> GetByIdWithLinesAsync(int id)
        {
            return await _context.PurchaseOrders
                .Include(p => p.Lines)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}