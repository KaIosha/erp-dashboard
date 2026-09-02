using backend.Data;
using backend.models;
using backend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.repositories.Implementations
{
    public class ProductRepository : GenericRepository<Products>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsBySkuAsync(string sku)
        {
            return await _context.Products.AnyAsync(p => p.SKU == sku);
        }

        public async Task<Products?> GetActiveByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<List<Products>> GetByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids.ToList();
            return await _context.Products.Where(p => idList.Contains(p.Id)).ToListAsync();
        }

        public async Task<(List<Products> Data, int TotalCount)> GetPageAsync(string? category, bool lowStock, int page, int pageSize)
        {
            var query = _context.Products.AsNoTracking().Include(p => p.Category).Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category.Name == category);
            }

            if (lowStock)
            {
                query = query.Where(p => p.StockQuantity <= p.ReorderLevel);
            }

            var totalCount = await query.CountAsync();
            var data = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, totalCount);
        }
    }
}