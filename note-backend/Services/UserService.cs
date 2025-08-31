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

        public async Task<User?> GetUserByRefreshToken(string? refreshToken)
        {
            if (refreshToken == null)
            {
                return null;
            }

            var user = await _userRepo.GetUserByRefreshToken(refreshToken);
            return user;
        }
    }
}
