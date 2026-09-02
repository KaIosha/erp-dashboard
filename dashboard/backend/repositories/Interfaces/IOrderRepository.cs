using backend.models;

namespace backend.repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Orders>
    {
        Task<(List<Orders> Data, int TotalCount)> GetPageAsync(string? search, string? status, int? customerId, DateTime? from, DateTime? to, int page, int pageSize);
        Task<Orders?> GetByIdWithLinesAsync(int id);
        Task<Orders?> GetByIdWithLinesAndInvoiceAsync(int id);
    }
}