// Controllers/NotesController.cs
using Microsoft.AspNetCore.Mvc;
using note_backend.Models;
using note_backend.Repositories;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly NoteRepository _repo;

    public NotesController(NoteRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _repo.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var note = await _repo.GetByIdAsync(id);
        return note == null ? NotFound() : Ok(note);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Note note)
    {
        await _repo.CreateAsync(note);
        return CreatedAtAction(nameof(Get), new { id = note.Id }, note);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Note note)
    {
        note.Id = id;
        await _repo.UpdateAsync(note);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}
