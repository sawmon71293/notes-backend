namespace note_backend.DTOs
{
    public class NoteResponseDTO
    {
        public string Title { get; set; } = "";
        public int Id { get; set; }
        public string? Content { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
    }
}
