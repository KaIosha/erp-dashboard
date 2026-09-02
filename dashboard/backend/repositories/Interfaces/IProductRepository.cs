using backend.models;

namespace backend.repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Products>
    {
        Task<bool> ExistsBySkuAsync(string sku);
        Task<Products?> GetActiveByIdAsync(int id);
        Task<List<Products>> GetByIdsAsync(IEnumerable<int> ids);
        Task<(List<Products> Data, int TotalCount)> GetPageAsync(string? category, bool lowStock, int page, int pageSize);
    }
}