using backend.models;

namespace backend.repositories.Interfaces
{
    public interface IDepartmentsRepository : IGenericRepository<Departments>
    {
        Task<List<Departments>> GetAllOrderedByNameAsync();
    }
}