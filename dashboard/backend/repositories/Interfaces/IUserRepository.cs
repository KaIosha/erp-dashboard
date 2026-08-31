using backend.models;

namespace backend.repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<Users>
    {
        Task<Users> FindEmailAsync(string email);
        Task<bool> AddUserAsync(Users user);
       
    }
}
