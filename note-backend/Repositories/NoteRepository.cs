using Dapper;
using Microsoft.Data.SqlClient;
using note_backend.Models;

namespace note_backend.Repositories
{
    public class NoteRepository
    {
        private readonly IConfiguration _config;

        public NoteRepository(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection => new SqlConnection(
            _config.GetConnectionString("DefaultConnection"));

        public async Task<IEnumerable<Note>> GetAllAsync()
        {
            using var conn = Connection;
            return await conn.QueryAsync<Note>("SELECT * FROM Notes");
        }

        public async Task<Note?> GetByIdAsync(int id)
        {
            using var conn = Connection;
            return await conn.QueryFirstOrDefaultAsync<Note>(
                "SELECT * FROM Notes WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> CreateAsync(Note note)
        {
            using var conn = Connection;
            var sql = @"INSERT INTO Notes (Title, Content)
                    VALUES (@Title, @Content)";
            return await conn.ExecuteAsync(sql, note);
        }

        public async Task<int> UpdateAsync(Note note)
        {
            using var conn = Connection;
            var sql = @"UPDATE Notes SET Title = @Title, Content = @Content WHERE Id = @Id";
            return await conn.ExecuteAsync(sql, note);
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var conn = Connection;
            return await conn.ExecuteAsync("DELETE FROM Notes WHERE Id = @Id", new { Id = id });
        }
    }
}
