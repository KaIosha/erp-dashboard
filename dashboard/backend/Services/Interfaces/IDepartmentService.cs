using backend.dtos;
using backend.models;

namespace backend.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<GetDepartmentDto>> GetAllDepartmentsAsync();
       
    }
}
