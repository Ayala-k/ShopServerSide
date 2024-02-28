using Microsoft.AspNetCore.Mvc;
using serverSide.Models;
using serverSide.Utils;
using System.Collections.Generic;

namespace serverSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            string query = "SELECT * FROM users";
            List<User> users = DbUtils.ExecuteSelectQuery<User>(query);
            return Ok(users);
        }


        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            string query = $"SELECT * FROM users WHERE Id={id}";
            List<User> user = DbUtils.ExecuteSelectQuery<User>(query);
            return Ok(user);
        }
        

        [HttpPost("signup")]
        public IActionResult SignUp(User user)
        {
            string hashedPassword= PasswordHashUtil.HashPassword(user.Password);
            string query = $"INSERT INTO users (Id, Name, Role, Password) VALUES ('{user.Id}', '{user.Name}', '{user.Role}','{hashedPassword}')";
            DbUtils.ExecuteNonQuery(query);
            return Ok("User added successfully");
        }

        [HttpPost("login")]
        public IActionResult LogIn(UserLogIn user)
        {
            string hashedPassword = PasswordHashUtil.HashPassword(user.Password);
            string query = $"SELECT * FROM users WHERE Name='{user.Name}'";
            List<User> userDetails = DbUtils.ExecuteSelectQuery<User>(query);
            if (hashedPassword == userDetails[0].Password)
            {
                return Ok("User logged in successfully");
            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpPut("{id}")]
        public IActionResult UpdateUser(string id, User user)//not includes password change!
        {
            string query = $"UPDATE users SET Name='{user.Name}', Role='{user.Role}' WHERE Id='{id}'";
            DbUtils.ExecuteNonQuery(query);
            return Ok("User updated successfully");
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteUser(string id)
        {
            string query = $"DELETE FROM users WHERE Id='{id}'";
            DbUtils.ExecuteNonQuery(query);
            return Ok("User deleted successfully");
        }
    }
}
