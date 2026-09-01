using backend.dtos;

namespace backend.Services.Interfaces
{
    public interface IRolesService
    {
        Task<PaginationResultDto<GetRoleDataDto>> GetAllRoles(int page = 1, int pageSize = 20);
        Task<GetRoleDataDto> GetRoleDataByIdAsync(int id);
        Task<GetRoleDataDto> UpdateRoleData(int id, UpdateRoleDataDto dto);
        Task<bool> DeleteRole(int id);
        Task<GetRoleDataDto> CreateRoleAsync(CreateRoleDto dto);
    }
}
