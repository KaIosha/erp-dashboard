using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using backend.UOW;

namespace backend.Services.Implementations
{
    public class RolesService : IRolesService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RolesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetRoleDataDto> CreateRoleAsync(CreateRoleDto dto)
        {
            var existingRole = await _unitOfWork.Roles.SingleOrDefaultAsync(x => x.Name == dto.Name);
            if (existingRole is not null)
            {
                return await Task.FromResult<GetRoleDataDto>(null);
            }

            var role = new Roles
            {
                Name = dto.Name,
                Permissions = dto.Permissions
            };
            await _unitOfWork.Roles.AddAsync(role);
            await _unitOfWork.SaveChangesAsync();

            return new GetRoleDataDto
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = role.Permissions
            };
        }

        public async Task<bool> DeleteRole(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role is null)
                return false;

            _unitOfWork.Roles.Remove(role);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<PaginationResultDto<GetRoleDataDto>> GetAllRoles(int page = 1, int pageSize = 20)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 20;

            var (roles, totalCount) = await _unitOfWork.Roles.GetPageAsync(page, pageSize);
            var data = roles.Select(r => new GetRoleDataDto
            {
                Id = r.Id,
                Name = r.Name,
                Permissions = r.Permissions
            }).ToList();

            return new PaginationResultDto<GetRoleDataDto>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
        public async Task<GetRoleDataDto> GetRoleDataByIdAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role is not null)
            {
                return new GetRoleDataDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Permissions = role.Permissions
                };
            }
            return null;
        }

        public async Task<GetRoleDataDto> UpdateRoleData(int id, UpdateRoleDataDto dto)
        {
            var role = await _unitOfWork.Roles.SingleOrDefaultAsync(r => r.Id == id);

            if (role is null)
                return null;

            role.Name = dto.Name ?? role.Name;
            role.Permissions = dto.Permissions ?? role.Permissions;

            _unitOfWork.Roles.Update(role);
            await _unitOfWork.SaveChangesAsync();

            return new GetRoleDataDto
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = role.Permissions
            };
        }
    }
}