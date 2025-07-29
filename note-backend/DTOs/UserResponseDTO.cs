using note_backend.Models;

namespace note_backend.DTOs
{
    public class UserResponseDTO
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public int Id { get; set; }
    }
}
