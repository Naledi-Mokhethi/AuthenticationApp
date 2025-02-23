using AuthenticationApp.Models;
using AuthenticationApp.UserDTO;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly string? _connectionString;
        private IConfiguration _configuration; //This is for the key config, lets see if it still works when its here

        public AuthService(string? connectionString, IConfiguration configuration)
        {
            _connectionString = connectionString;
            _configuration = configuration;
        }

        public async Task<string?> RegisterAsync(UserDto request)
        {
            var user = new User(); 

            var hashedPassword = new PasswordHasher<User>()
              .HashPassword(user, request.EmployeePassword);

            var procedureName = "EmployeeRegistration";
            var parameters = new DynamicParameters();
            using (var connection = new SqlConnection(_connectionString))
            {
                parameters.Add("@EmployeeName", request.EmployeeName);
                parameters.Add("@EmployeeLastName", request.EmployeeLastName);
                parameters.Add("@EmployeeDepartment", request.EmployeeDepartment);
                parameters.Add("@EmloyeeeJobTitle", request.EmloyeeeJobTitle);
                parameters.Add("@EmployeePhoneNumber", request.EmployeePhoneNumber);
                parameters.Add("@EmployeeEmail", request.EmployeeEmail);
                parameters.Add("@EmployeeHashedPassword", hashedPassword);
                var data = await connection.QueryAsync<User>(procedureName, parameters, commandType: CommandType.StoredProcedure);
                if (data is not null)
                    return "Success";
                else
                    return null;
            }
        }
        public async Task<string> LoginAsync(LoginUserDto request)
        {
            var loginProcedure = "EmployeeLogin";
            var passwordProcedureName = "ReturnHashedPassword";
            var parameters = new DynamicParameters();
            var passordParameters = new DynamicParameters();

            using (var connection = new SqlConnection(_connectionString))
            {
                //connection.Open();

                //Password Stored Procedure 
                passordParameters.Add("@Email", request.EmployeeEmail);
                var hashedPassword = await connection.QueryFirstOrDefaultAsync<string>(passwordProcedureName, passordParameters, commandType: CommandType.StoredProcedure);
                if (hashedPassword is null)
                {
                    return null!; // intentionall null return, it means that there is no user with that email in the dbo
                }

                if (new PasswordHasher<User>().VerifyHashedPassword(new User(), hashedPassword, request.EmployeePassword)
                   == PasswordVerificationResult.Failed)
                {
                    return null!; // Invalid password
                }
                //If checks pass, we execute the login stored procedure, everything should work here
                parameters.Add("@Email", request.EmployeeEmail);  
                parameters.Add("@Password", hashedPassword.ToString()!);//pass in hashed password since thats whats in the Dbo 
                var user = await connection.QueryFirstOrDefaultAsync(loginProcedure, parameters, commandType:CommandType.StoredProcedure);
                if (user is not null)
                    return CreateToken(request);
                else
                    return null!;  
            }
    
        }
        private string CreateToken(LoginUserDto user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.EmployeeEmail)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings: Audiance"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

    }
}
