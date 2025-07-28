using Dapper;
using Microsoft.Data.SqlClient;
using note_backend.Models;

namespace note_backend.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        private SqlConnection Connection => new SqlConnection(_connectionString);

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var conn = Connection;
            return await conn.QueryAsync<User>("SELECT * FROM Users");
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var conn = Connection;
            return await conn.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> CreateAsync(User user)
        {
            using var conn = Connection;
            var sql = "INSERT INTO Users (Name, Email, Password) VALUES (@Name, @Email, @Password)";
            return await conn.ExecuteAsync(sql, user);
        }

        public async Task<int> UpdateAsync(User user)
        {
            using var conn = Connection;
            var sql = "UPDATE Users SET Name = @Name, Email = @Email, Password = @Password WHERE Id = @Id";
            return await conn.ExecuteAsync(sql, user);
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var conn = Connection;
            return await conn.ExecuteAsync("DELETE FROM Users WHERE Id = @Id", new { Id = id });
        }
    }
}
