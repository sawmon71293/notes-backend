using Microsoft.IdentityModel.Tokens;
using note_backend.Models;
using note_backend.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace note_backend.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;
        private readonly TokenRepository _tokenRepository;

        public AuthService(IConfiguration config, TokenRepository tokenRepository)
        {
            _config = config;
            _tokenRepository = tokenRepository;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };
            var jwtKey = _config["JwtSettings:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RefreshToken> GenerateRefreshToken(User user)
        {
            var randomNumber = new Byte[32];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            
            var token = Convert.ToBase64String(randomNumber);
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };
            await _tokenRepository.CreateAsync(refreshToken);
            return refreshToken;
            
            
        }
    }
}
