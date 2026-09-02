using backend.models;

namespace backend.repositories.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employees>
    {
        Task<(List<Employees> Data, int TotalCount)> GetPageAsync(string? search, string? department, int page, int pageSize);
        Task<Employees?> GetActiveByIdAsync(int id);
    }
}