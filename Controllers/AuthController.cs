using AuthenticationApp.Models;
using AuthenticationApp.Services;
using AuthenticationApp.UserDTO;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            return Ok(user);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginUserDto request)
        {
            var token = await authService.LoginAsync(request);
            if (token == null)
            {
                return BadRequest("");
            }
            return Ok(token);
        }

    }
}
