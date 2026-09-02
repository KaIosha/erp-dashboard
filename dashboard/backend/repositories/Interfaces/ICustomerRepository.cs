using backend.models;

namespace backend.repositories.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customers>
    {
        Task<bool> ExistsByEmailOrPhoneAsync(string email, string phone);
        Task<Customers?> GetActiveByIdAsync(int id);
        Task<(List<Customers> Data, int TotalCount)> GetPageAsync(int page, int pageSize);
    }
}