using Dapper;
using Microsoft.Data.SqlClient;
using note_backend.Models;
using System.Data;
using note_backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace note_backend.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _config;
        public UserRepository(IConfiguration config)
        {
            _config = config;
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

        public async Task<IEnumerable<Note>> GetNotesByUserIdAsync(int userId)
        {
            using var conn = Connection;
            var sql = "SELECT * FROM Notes WHERE UserId = @UserId";
            return await conn.QueryAsync<Note>(sql, new { UserId = userId });
        }


        public async Task<User?> GetByEmailAsync(string email)
        {
            using var conn = Connection;
            return await conn.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Email = @Email", new { Email = email });
        }

        public async Task<int> CreateAsync(User user)
        {

            try
            {
                using var conn = Connection;

                var sql = @"INSERT INTO Users (Name, Email, Password)
                         OUTPUT INSERTED.Id
                         VALUES (@Name, @Email, @Password)";
                var newUserId = await conn.ExecuteScalarAsync<int>(sql, new
                {
                    Name = user.Name,
                    Email = user.Email,
                    Password = user.Password,
                    CreatedAt = DateTime.UtcNow
                });

                return newUserId;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
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

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            using var conn = Connection;
            var sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { Email = email });
            return count > 0;
        }

        public async Task<bool> IsDatabaseConnectedAsync()
        {
            try
            {
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();
                return connection.State == ConnectionState.Open;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database connection error: " + ex.Message);
                return false;
            }
        }

        public async Task<RefreshToken?> GetUserByRefreshToken(string? refreshToken)
        {
            using var connection = Connection;
            var sql = "SELECT * FROM RefreshTokens WHERE Token = @Token";
            return await connection.QueryFirstOrDefaultAsync(sql, new { Token = refreshToken });
            
        }
    
        
    }
}
