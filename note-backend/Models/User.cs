namespace note_backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
