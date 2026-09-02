using backend.Data;
using backend.models;
using backend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.repositories.Implementations
{
    public class InvoiceRepository : GenericRepository<Invoices>, IInvoiceRepository
    {
        public InvoiceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(List<Invoices> Data, int TotalCount)> GetPageAsync(string? search, string? status, int? customerId, DateTime? from, DateTime? to, int page, int pageSize)
        {
            IQueryable<Invoices> query = _context.Invoices.AsNoTracking().Include(i => i.Customer).Include(i => i.Lines);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(i =>
                    i.Customer.Name.ToLower().Contains(search) ||
                    i.Lines.Any(l => l.Description.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(i => i.Status == status);

            if (customerId.HasValue)
                query = query.Where(i => i.CustomerId == customerId.Value);

            if (from.HasValue)
                query = query.Where(i => i.InvoiceDate >= from.Value);

            if (to.HasValue)
                query = query.Where(i => i.InvoiceDate <= to.Value);

            var totalCount = await query.CountAsync();
            var data = await query
                .OrderByDescending(i => i.InvoiceDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, totalCount);
        }

        public async Task<Invoices?> GetByIdWithLinesAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Invoices?> GetByIdWithCustomerAndLinesAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}