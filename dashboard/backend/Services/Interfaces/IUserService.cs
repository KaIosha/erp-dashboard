using backend.dtos;
using backend.models;

namespace backend.Services.Interfaces
{
    public interface IUserService
    {
        Task<PaginationResultDto<GetUserDataDto>> GetAllUsers(int page = 1, int pageSize = 20);
        Task<GetUserDataDto> GetUserDataByIdAsync(int id);
        Task<GetUserDataDto> UpdateUserData(int id, UpdateUserDataDto dto);
        Task<bool> DeleteUser(int id);
        Task<GetUserDataDto> CreateUserAsync(CreateUserDto dto);
    }
}
