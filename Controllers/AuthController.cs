using AuthenticationApp.Models;
using AuthenticationApp.UserDTO;
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
    public class AuthController(IConfiguration configuration) : ControllerBase
    {
        //Static user, just to test out the API
        //Seeing if we need the Dto
        public static User user = new (); //testing only 
        [HttpPost("Register")]
        public ActionResult<User> Register(UserDto request)
        {
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.EmployeePassword);
            //mappring the data
            user.EmployeeEmail = request.EmployeeEmail;
            user.EmployeeHashedPassword = hashedPassword;
            user.EmployeeName = request.EmployeeName;
            return Ok(user);
        }
        [HttpPost("Login")]
        public ActionResult<string> Login(LoginUserDto request)
        {
            if(user.EmployeeEmail != request.EmployeeEmail)
            {
                return BadRequest("User not found");
            }
            if(new PasswordHasher<User>().VerifyHashedPassword(user, user.EmployeeHashedPassword, request.EmployeePassword) 
                == PasswordVerificationResult.Failed)
            {
                return BadRequest("Wrong passord");
            }
            string token = CreateToken(user);
            return Ok(token);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.EmployeeEmail)
             };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings: Audiance"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
