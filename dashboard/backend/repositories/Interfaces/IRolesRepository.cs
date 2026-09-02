using backend.models;

namespace backend.repositories.Interfaces
{
    public interface IRolesRepository : IGenericRepository<Roles>
    {
        Task<(List<Roles> Data, int TotalCount)> GetPageAsync(int page, int pageSize);
    }
}