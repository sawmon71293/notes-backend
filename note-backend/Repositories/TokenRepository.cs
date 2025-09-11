using Dapper;
using Microsoft.Data.SqlClient;
using note_backend.Models;

namespace note_backend.Repositories
{
    public class TokenRepository
    {

        private readonly IConfiguration _config;

        public TokenRepository(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection => new SqlConnection(
          _config.GetConnectionString("DefaultConnection"));

        public async Task CreateAsync (RefreshToken token)
        {
            using var con = Connection;
            var sql = @"INSERT INTO RefreshTokens (UserId, Token, ExpiresAt, CreatedAt, Revoked ) 
                      VALUES (@UserId, @Token, @ExpiresAt, @CreatedAt, @Revoked)";
            await con.ExecuteAsync(sql, token);
        }

        public async Task<int> RevokeAsync(string token)
        {
            using var con = Connection;
            var sql = "UPDATE RefreshTokens SET Revoked=1 WHERE Token = @Token";
            return await con.ExecuteAsync(sql, new { Token = token });
        }
    }
}
