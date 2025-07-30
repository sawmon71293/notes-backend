// Controllers/NotesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using note_backend.DTOs;
using note_backend.Models;
using note_backend.Repositories;
using System.Collections;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

[ApiController]
[Route("api/")]
[Authorize]
public class NoteController : ControllerBase
{
    private readonly NoteRepository _repo;
    private readonly UserRepository _userRepo;

    public NoteController(NoteRepository repo, UserRepository userRepo)
    {
        _repo = repo;
        _userRepo = userRepo;
    }

    [HttpGet("note")]
    public async Task<IActionResult> Get()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        User? user = await _userRepo.GetByEmailAsync(email);
        return Ok(await _repo.GetAllAsync(userId: user.Id));
    }

    [HttpGet("note/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var note = await _repo.GetByIdAsync(id);
        return note == null ? NotFound() : Ok(note);
    }

    [HttpPost("note")]
    public async Task<IActionResult> Post([FromBody] NoteCreateDTO noteDto)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        User? user = await _userRepo.GetByEmailAsync(email);
        
        if (user == null)
        {
            return Unauthorized(new { message = "User not found" });
        }

        Note? note = new Note
        {
            Title = noteDto.Title,
            Content = noteDto.Content,
            CreatedAt = DateTime.UtcNow,
            UserId = user.Id
        };

        var noteId = await _repo.CreateAsync(note); // Modify to return the new ID

        var response = new NoteResponseDTO
        {
            Id = noteId,
            Title = note.Title,
            Content = note?.Content,
            CreatedAt = note.CreatedAt,
            UserId = user.Id
        };

        return Ok(response);
    }

    [HttpPut("note/{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] NoteCreateDTO noteDto)
    {
        NoteUpdateDTO updateDto = new NoteUpdateDTO
        {
            Id = id,
            Title = noteDto.Title,
            Content = noteDto.Content,
            UpdatedAt = DateTime.UtcNow

        };
        await _repo.UpdateAsync(updateDto);
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        User? user = await _userRepo.GetByEmailAsync(email);
        return Ok(await _repo.GetAllAsync(userId: user.Id));
    }

    [HttpDelete("note/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        User? user = await _userRepo.GetByEmailAsync(email);
        return Ok(await _repo.GetAllAsync(userId: user.Id));
    }

    [HttpGet("notes")]
    public async Task<IEnumerable<Note>> getNotes(
        [FromQuery] string? query,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool descending = false
        )
    {
        var querying = new NoteQueryDTO
        {
            OrderBy = sortBy,
            OrderbyDescending = descending,
            Query = query
        };
        return (await _repo.GetAllByQueryAsync(querying));
    }
}
