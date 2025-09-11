using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using note_backend.DTOs;
using note_backend.Models;
using note_backend.Repositories;
using note_backend.Services;
using System.Linq;
using System.Security.Claims;

namespace note_backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepo;
        private readonly UserService _userService;
        private readonly AuthService _authService;
        private List<User> _users = new List<User>();

        public AuthController(UserRepository repo, AuthService authService, UserService userService)
        {
            _userRepo = repo;
            _authService = authService;
            _users = _userRepo.GetAllAsync().Result.ToList();
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO dto)
        {
            if (_users.Any(u => u.Email == dto.Email))
                return BadRequest("User already exists");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = passwordHash
            };

            int createdId = await _userRepo.CreateAsync(user);
            UserResponseDTO userResponseDTO = new UserResponseDTO
            {
                Id = createdId,
                Email = dto.Email,
                Name = dto.Name,
            };

            return Ok(userResponseDTO);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(UserLoginDTO dto)
        {
            var user = _users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null || string.IsNullOrEmpty(user.Password) || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Unauthorized("Invalid email or password");


            var token =  _authService.GenerateJwtToken(user);
            var refreshToken = await _authService.GenerateRefreshToken(user);

            //Response.Cookies.Append("refresh_token", refreshToken.Token, new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = true,
            //    SameSite = SameSiteMode.None,
            //    Expires = DateTime.UtcNow.AddDays(7),
            //    Path="/"
            //});

            return Ok(new { token, user, refreshToken = refreshToken.Token });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var user = await _userRepo.GetByEmailAsync(email); // Or however your user lookup works

            if (user == null)
                return NotFound();

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token, user });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer"))
            {
                return Unauthorized("Invalid Authorization header");
            }
            var refreshToken = authHeader.Substring("Bearer ".Length).Trim();
            var user = await _userService.GetUserByRefreshToken(refreshToken);

            if (user == null)
            {
                return Unauthorized();
            }
                
            string newAccessToken = _authService.GenerateJwtToken(user);
            var newRefreshToken = await _authService.GenerateRefreshToken(user);

            //Response.Cookies.Append("refresh_token", newRefreshToken.Token, new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = true,
            //    SameSite = SameSiteMode.None,
            //    Expires = DateTime.UtcNow.AddDays(7),
            //    Path= "/"
            //});
            return Ok(new { user, accessToken = newAccessToken, refreshToken = newRefreshToken.Token });
        }
    }
}
