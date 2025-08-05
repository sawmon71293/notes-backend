namespace note_backend.DTOs
{
    public class NoteQueryDTO
    {
        public string? OrderBy { get; set; }
        public bool OrderbyDescending { get; set; }
        public string? Query { get; set; }

        public int UserId { get; set; }
    }
}
