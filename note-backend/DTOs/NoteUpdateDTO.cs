namespace note_backend.DTOs
{
    public class NoteUpdateDTO
    {
        public string Title { get; set; } = " ";
        public string? Content { get; set; } = "";
        public int Id { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
