using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using backend.UOW;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginationResultDto<GetUserDataDto>> GetAllUsers(int page = 1,int pageSize = 20)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 20;

            var query = _unitOfWork.Repository<Users>().GetQueryable();

            var totalCount = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new GetUserDataDto
                {
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    RoleId = p.RoleId
                })
                .ToListAsync();

            return new PaginationResultDto<GetUserDataDto>
            {
                Data = products,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
        public async Task<GetUserDataDto> GetUserDataByIdAsync(int id)
        {
            var User = await _unitOfWork.Users.GetByIdAsync(id);
            if (User is not null)
            {
                return new GetUserDataDto
                {
                    FirstName = User.FirstName,
                    LastName = User.LastName,
                    Email = User.Email,
                    RoleId = User.RoleId
                };
            }

            return null;
        }
        public async Task<GetUserDataDto> UpdateUserData(int id, UpdateUserDataDto dto)
        {
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (user is null)
                return null;

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new GetUserDataDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleId = user.RoleId
            };
        }
        public async Task<bool> DeleteUser(int id)
        {
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(x => x.Id == id);
            if (user is null)
            {
                return false;
            }

            var tokens = await _unitOfWork.RefreshTokenRepository.FindAsync(u => u.UserId == id);

            foreach (var item in tokens)
            {
                _unitOfWork.RefreshTokenRepository.Remove(item);
            }
            _unitOfWork.Users.Remove(user);
            await _unitOfWork.SaveChangesAsync();
            return true;

        }
        public async Task<GetUserDataDto> CreateUserAsync(CreateUserDto dto)
        {

            var existingUser = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser is not null)
            {
                return await Task.FromResult<GetUserDataDto>(null);
            }

            var passwordHasher = new PasswordHasher<Users>();
            var user = new Users
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                RoleId = dto.RoleId
            };
            user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new GetUserDataDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleId = user.RoleId
            };
        }
    }
}
