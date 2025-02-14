using AuthenticationApp.Models;
using AuthenticationApp.UserDTO;

namespace AuthenticationApp.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<string> LoginAsync(UserDto request);//Temp
    }
}
