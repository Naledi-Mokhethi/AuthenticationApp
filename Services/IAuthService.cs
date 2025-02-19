using AuthenticationApp.Models;
using AuthenticationApp.UserDTO;

namespace AuthenticationApp.Services
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(UserDto request);
        Task<string> LoginAsync(LoginUserDto request);//Temp
       // Task<LoginUserDto> LoginAsync1(LoginUserDto loginRequest);//Perm
    }
}
