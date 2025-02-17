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
        private IConfiguration? configuration; //This is for the key config, lets see if it still works when its here

        public AuthService(string? connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<User?> RegisterAsync(UserDto request) // This method to be modified to void, we don't need to return the user, or just return a string that says success
        {
            var user = new User();

            var hashedPassword = new PasswordHasher<User>()
              .HashPassword(user, request.EmployeePassword);

            //mapping the data, will be moved obviously 
            user.EmployeeName = request.EmployeeName;
            user.EmployeeLastName = request.EmployeeLastName;
            user.EmployeeDepartment = request.EmployeeDepartment;
            user.EmloyeeeJobTitle = request.EmloyeeeJobTitle;
            user.EmployeePhoneNumber = request.EmployeePhoneNumber;
            user.EmployeeEmail = request.EmployeeEmail;
            user.EmployeeHashedPassword = hashedPassword;

            var procedureName = "EmployeeRegistration";
            var parameters = new DynamicParameters();
            using(var connection = new SqlConnection( _connectionString))
            {
                connection.Open();
               // parameters.Add("Name", request.EmployeeName);
                parameters.AddDynamicParams(user);//trying it out
                var data = await connection.QueryAsync<User>(procedureName, parameters, commandType: CommandType.StoredProcedure);
                return (User?)data; //Might not need to return anything
            
            }
          //  return Ok(user);
        }
        public async Task<string> LoginAsync(LoginUserDto request)
        {
            var loginProcedure = "EmployeeLogin";
            var passwordProcedureName = "ReturnHashedPassword";
            var parameters = new DynamicParameters();
            var passordParameters = new DynamicParameters();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
               
                //Password Stored Procedure 
                var hashedPassword = await connection.QueryFirstOrDefaultAsync<LoginUserDto>(passwordProcedureName, parameters, commandType: CommandType.StoredProcedure);
                if (hashedPassword is null)
                {
                    return null!; // intentionall null return, it means that there is no user with that email in the dbo
                }
                 
                if( new PasswordHasher<LoginUserDto>().VerifyHashedPassword(request,hashedPassword.ToString()!, request.EmployeePassword) == PasswordVerificationResult.Failed)
                {
                    return null!; //Return Null if the password being passed in does not match the hashed one in the dbo
                }
                //If checks pass, we execute the login stored procedure, everything should work here
                parameters.Add("Email", request.EmployeeEmail);
                parameters.Add("@Password", hashedPassword.ToString()!);//pass in hashed password since thats whats in the Dbo 
                var user = await connection.QueryFirstOrDefaultAsync<LoginUserDto>(loginProcedure, parameters, commandType:CommandType.StoredProcedure);
                connection.Close();
                return CreateToken(user!);
            }
    
        }
        private string CreateToken(LoginUserDto user)
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
