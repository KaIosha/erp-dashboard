using backend.Data;
using backend.models;
using backend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.repositories.Implementations
{
    public class OrderRepository : GenericRepository<Orders>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(List<Orders> Data, int TotalCount)> GetPageAsync(string? search, string? status, int? customerId, DateTime? from, DateTime? to, int page, int pageSize)
        {
            IQueryable<Orders> query = _context.Orders.AsNoTracking().Include(o => o.Lines);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(o =>
                    o.Customer.Name.ToLower().Contains(search) ||
                    o.PaymentMethod.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(o => o.Status == status);

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId.Value);

            if (from.HasValue)
                query = query.Where(o => o.OrderDate >= from.Value);

            if (to.HasValue)
                query = query.Where(o => o.OrderDate <= to.Value);

            var totalCount = await query.CountAsync();
            var data = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, totalCount);
        }

        public async Task<Orders?> GetByIdWithLinesAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Lines)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Orders?> GetByIdWithLinesAndInvoiceAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Invoice)
                .Include(o => o.Lines)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}