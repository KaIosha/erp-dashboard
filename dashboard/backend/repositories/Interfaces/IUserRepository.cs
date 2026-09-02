using backend.models;

namespace backend.repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<Users>
    {
        Task<Users?> FindEmailAsync(string email);
        Task<bool> AddUserAsync(Users user);
        Task<(List<Users> Data, int TotalCount)> GetPageAsync(int page, int pageSize);
    }
}
