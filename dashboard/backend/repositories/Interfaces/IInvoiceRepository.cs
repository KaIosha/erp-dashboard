using backend.models;

namespace backend.repositories.Interfaces
{
    public interface IInvoiceRepository : IGenericRepository<Invoices>
    {
        Task<(List<Invoices> Data, int TotalCount)> GetPageAsync(string? search, string? status, int? customerId, DateTime? from, DateTime? to, int page, int pageSize);
        Task<Invoices?> GetByIdWithLinesAsync(int id);
        Task<Invoices?> GetByIdWithCustomerAndLinesAsync(int id);
    }
}