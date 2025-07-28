using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using note_backend.DTOs;
using note_backend.Models;
using note_backend.Repositories;
using note_backend.Services;
using System.Security.Claims;

namespace note_backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _repo;
        private readonly AuthService _authService;
        private List<User> _users = new List<User>();

        public AuthController(UserRepository repo, AuthService authService)
        {
            _repo = repo;
            _authService = authService;

            _users = _repo.GetAllAsync().Result.ToList();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (_users.Any(u => u.Email == dto.Email))
                return BadRequest("User already exists");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User
            {
                Id = _users.Count + 1,
                Name = dto.Name,
                Email = dto.Email,
                Password = passwordHash
            };

            await _repo.CreateAsync(user);
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginDto dto)
        {
            var user = _users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null || string.IsNullOrEmpty(user.Password) || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Unauthorized("Invalid email or password");


            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _users.FirstOrDefault(u => u.Id.ToString() == userId);
            if (user == null) return NotFound();

            return Ok(new { user.Id, user.Name, user.Email });
        }
    }
}
