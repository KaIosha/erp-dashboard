using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Config;
using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using backend.UOW;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwt;
        public JwtService(IUnitOfWork unitOfWork, IOptions<JwtSettings> jwt)
        {
            _unitOfWork = unitOfWork;
            _jwt = jwt.Value;
        }
        public async Task<JwtDto> GetJwtAsync(Users user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName+" "+ user.LastName!),
                new Claim(JwtRegisteredClaimNames.Sub, user.FirstName + user.LastName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role,user.Role.Name.ToString())
            };

            if (user.Role?.Permissions is { Count: > 0 })
            {
                foreach (var permission in user.Role.Permissions)
                {
                    claims.Add(new Claim("permission", permission));
                }
            }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));

             var token = new JwtSecurityToken(
             issuer: _jwt.Issuer,
             audience: _jwt.Audience,
             claims: claims,
             expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpiryMinutes),
             signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
         );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString(),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new JwtDto
            {
                JwtToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpireAt = token.ValidTo
            };
        }
    }
}
