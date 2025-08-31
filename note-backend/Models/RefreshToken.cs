namespace note_backend.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Revoked { get; set; } = false;

        public override string ToString()
        {
            return $"Token={Token}, UserId={UserId}, ExpiresAt={ExpiresAt}";
        }
    }
}
