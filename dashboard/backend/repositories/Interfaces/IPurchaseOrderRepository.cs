using backend.models;

namespace backend.repositories.Interfaces
{
    public interface IPurchaseOrderRepository : IGenericRepository<PurchaseOrders>
    {
        Task<(List<PurchaseOrders> Data, int TotalCount)> GetPageAsync(string? search, int page, int pageSize);
        Task<PurchaseOrders?> GetByIdWithLinesAsync(int id);
    }
}