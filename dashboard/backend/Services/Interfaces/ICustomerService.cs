using backend.dtos;

namespace backend.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<PaginationResultDto<GetCustomerDataDto>> GetAllCustomers(int page = 1, int pageSize = 20);
        Task<GetCustomerDataDto> GetCustomerDataByIdAsync(int id);
        Task<GetCustomerDataDto> UpdateCustomerData(int id, UpdateCustomerDataDto dto);
        Task<bool> DeleteCustomer(int id);
        Task<GetCustomerDataDto> CreateCustomerAsync(CreateCustomerDto dto);
    }
}
