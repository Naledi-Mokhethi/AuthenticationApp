using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace AuthenticationApp.Models
{
    public class User
    {
       public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeLastName { get; set; } = string.Empty;
        public string EmployeeDepartment { get; set; } = string.Empty;
        public string EmloyeeeJobTitle { get; set; } = string.Empty;
        public string EmployeePhoneNumber { get; set; } = string.Empty;
        public string EmployeeEmail { get; set; } = string.Empty;
        public string EmployeeHashedPassword {  get; set; } = string.Empty; 

    }
}
