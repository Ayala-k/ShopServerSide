using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using serverSide.Models;
using serverSide.Utils;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace serverSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {

            string query = "SELECT * FROM users";
            List<User> users = DbUtils.ExecuteSelectQuery<User>(query);
            return Ok(users);
        }


        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            string query = $"SELECT * FROM users WHERE Id={id}";
            List<User> user = DbUtils.ExecuteSelectQuery<User>(query);
            return Ok(user);
        }
        

        [HttpPost("signup")]
        public IActionResult SignUp(User user)
        {
            user.Role=Roles.Customer;
            string hashedPassword= PasswordHashUtil.HashPassword(user.Password);
            string query = $"INSERT INTO users (Name, Role, Password) VALUES ('{user.Name}', '{user.Role}','{hashedPassword}')";
            try
            {
                DbUtils.ExecuteNonQuery(query);
                return Ok("User added successfully");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                return Conflict("User name already exists");
            }
        }


        [HttpPost("login")]
        public IActionResult LogIn(UserLogIn user)
        {
            string hashedPassword = PasswordHashUtil.HashPassword(user.Password);
            string query = $"SELECT * FROM users WHERE Name='{user.Name}'";
            List<User> userDetails = DbUtils.ExecuteSelectQuery<User>(query);
            if (hashedPassword == userDetails[0].Password)
            {
                var token = GenerateJwtToken((int)userDetails[0].Id);
                return Ok(new { Token = token });
            }
            else
            {
                return Unauthorized("Wrong user name or password");
            }
        }

        private string GenerateJwtToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, User user)
        {
            string query = $"UPDATE users SET Name='{user.Name}', Role='{user.Role}' WHERE Id={id}";
            DbUtils.ExecuteNonQuery(query);
            return Ok("User updated successfully");
        }


        [HttpPut("changePassword/{id}")]
        public IActionResult ChangePassword(int id, [FromBody] string newPassword)
        {
            string hashedPassword = PasswordHashUtil.HashPassword(newPassword);
            string query = $"UPDATE users SET Password='{hashedPassword}' WHERE Id={id}";
            DbUtils.ExecuteNonQuery(query);
            return Ok("Password updated successfully");
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            string query = $"DELETE FROM users WHERE Id={id}";
            DbUtils.ExecuteNonQuery(query);
            return Ok("User deleted successfully");
        }
    }
}
