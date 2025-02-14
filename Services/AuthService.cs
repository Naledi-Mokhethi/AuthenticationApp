using AuthenticationApp.Models;
using AuthenticationApp.UserDTO;

namespace AuthenticationApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly string _connectionString;

        public AuthService(string connectionString)
        {
            _connectionString = connectionString;
        }
        public Task<string> LoginAsync(UserDto request)
        {
            throw new NotImplementedException();
        }

        public Task<User?> RegisterAsync(UserDto request)
        {
            throw new NotImplementedException();
        }
    }
}
