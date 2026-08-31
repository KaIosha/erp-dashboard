using Azure.Core;
using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using backend.UOW;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;

namespace backend.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private IUnitOfWork _unitOfWork;
        private IJwtService _jwt;
        public AuthService(IUnitOfWork unitofwork, IJwtService jwt)
        {
            _unitOfWork = unitofwork;
            _jwt = jwt;
        }


        public async Task<RegisterResponseDto> RegisterUserAsync(UserRegisterDto dto)
        {
            //// Step 1: Validate the DTO (FluentValidation rules from UserRegisterValidation)
            //var validationResult = await _registerValidator.ValidateAsync(dto);
            //if (!validationResult.IsValid)
            //{
            //    return new AuthResponseDto
            //    {
            //        IsAuthenticated = false,
            //        Message = "Validation failed: " + string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
            //    };
            //}

            var exsitingUser = await _unitOfWork.Users.FindEmailAsync(dto.Email);
            if (exsitingUser is not null)
            {

                return new RegisterResponseDto
                {
                    Message = "Email is already registered."
                };
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
            var CreatedUser = await _unitOfWork.Users.AddUserAsync(user);
            if (CreatedUser is false)
            {
                return new RegisterResponseDto
                {
                    IsSuccess = false,
                    Message = $"User creation failed"
                };
            }

            await _unitOfWork.SaveChangesAsync();
            return new RegisterResponseDto
            {
                IsSuccess = true,
                Message = "User registered successfully.",
                Email = user.Email,
            };
        }


        public async Task<LoginResponseDto> LoginAsync(UserLoginDto dto)
        {
            var user = await _unitOfWork.Users.FindEmailAsync(dto.Email);
            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var passwordHasher = new PasswordHasher<Users>();
            var checkPassword = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (checkPassword == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            var token = await _jwt.GetJwtAsync(user);

            return new LoginResponseDto
            {
                Token = token.JwtToken,
                RefreshToken = token.RefreshToken,
                ExpiresAt = token.ExpireAt,
            };
        }


        public async Task<LogOutDto> LogOut(string RefreshToken)
        {
            var Token = await _unitOfWork.Repository<RefreshToken>().SingleOrDefaultAsync(x => x.Token == RefreshToken);
            if (Token is null || Token.IsRevoked == true)
            {
                return new LogOutDto
                {
                    Message = "Refresh token doesn't exist in our DB"
                };
            }

            Token.IsRevoked = true;
            Token.ExpiresAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();
            return new LogOutDto
            {
                IsSuccess = true,
                Message = "LogOut Successfully"
            };
        }

        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var existing = await _unitOfWork.RefreshTokenRepository.FirstOrDefaultAsync(dto.RefreshToken);
            if (existing is null)
            {
                return new RefreshTokenResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid refresh token."
                };
            }

            if (existing.IsRevoked)
            {
                return new RefreshTokenResponseDto
                {
                    IsSuccess = false,
                    Message = "Refresh token has been revoked."
                };
            }

            if (existing.ExpiresAt <= DateTime.UtcNow)
            {
                return new RefreshTokenResponseDto
                {
                    IsSuccess = false,
                    Message = "Refresh token has expired."
                };
            }

            existing.IsRevoked = true;
            var token = await _jwt.GetJwtAsync(existing.User);
            await _unitOfWork.SaveChangesAsync();
            return new RefreshTokenResponseDto
            {
                IsSuccess = true,
                Message = "Token refreshed successfully.",
                JwtToken = token.JwtToken,
                RefreshToken = token.RefreshToken,
                ExpireAt = token.ExpireAt
            };
        }
    }
}
