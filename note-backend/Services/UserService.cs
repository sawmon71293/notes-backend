using note_backend.Models;
using note_backend.Repositories;

namespace note_backend.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepo;

        public UserService (UserRepository userRepository)
        {
            _userRepo = userRepository;
        }

        public Task<User?> GetUserByRefreshToken(string? refreshToken)
        {
            var refreshTokenData = _userRepo.GetUserByRefreshToken(refreshToken);
            Console.WriteLine(refreshTokenData.ToString());
            return Task.FromResult<User?>(new User { });
        }
    }
}
