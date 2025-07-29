using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using note_backend.DTOs;
using note_backend.Models;
using note_backend.Repositories;
using System.Data;
using System.Security.Claims;

namespace note_backend.Controllers
{
    [Route("api/user")]
    [ApiController]
    
    public class UserController : ControllerBase
    {
        private readonly UserRepository _repo;

        public UserController(UserRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return NotFound();
            UserResponseDTO userResponseDTO = new UserResponseDTO()
            {
                Email = user.Email,
                Name = user.Name,
                Id = user.Id,
            };
            return Ok(userResponseDTO);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDTO userDto)
        {
            var updatingUser = await _repo.GetByIdAsync(id);
            var loginnedEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (updatingUser == null) return NotFound();
            if (updatingUser.Email != loginnedEmail)
            {
                return Forbid("You are not allowed to edit someone else’s info.");
            }

            User user = new User
            {
                Id = id,
                Email = userDto.Email,
                Name = userDto.Name,
                Password = userDto.Password,
            };
            var result = await _repo.UpdateAsync(user);
            if (result > 0)
                return NoContent();

            return StatusCode(500, "An error occurred while updating the user.");

        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var loginnedEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var updatingUser = await _repo.GetByIdAsync(id);
            if(updatingUser == null) return NotFound();

            if (updatingUser.Email == loginnedEmail)
            {
                return Forbid("Cannot delete the user");
            }

            await _repo.DeleteAsync(id);
            return Ok();
        }

        [HttpGet("{id}/notes")]
        [Authorize]
        public async Task<IActionResult> GetUserNotes(int id)
        {
            var notes = await _repo.GetNotesByUserIdAsync(id);
            return Ok(notes);
        }


    }
}
