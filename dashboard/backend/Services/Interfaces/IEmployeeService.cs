using backend.dtos;

namespace backend.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<PaginationResultDto<GetEmployeeDto>> GetAllEmployeesAsync(
            string? search = null,
            string? department = null,
            int page = 1,
            int pageSize = 20);

        Task<GetEmployeeDto?> GetEmployeeByIdAsync(int id);

        Task<GetEmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto);

        Task<GetEmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);

        Task<bool> DeleteEmployeeAsync(int id);

    }
}