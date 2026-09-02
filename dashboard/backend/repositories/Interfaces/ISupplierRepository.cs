using backend.models;

namespace backend.repositories.Interfaces
{
    public interface ISupplierRepository : IGenericRepository<Suppliers>
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<(List<Suppliers> Data, int TotalCount)> GetPageAsync(string? search, int page, int pageSize);
    }
}