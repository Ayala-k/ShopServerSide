using Microsoft.AspNetCore.Authorization;
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
                return Ok("User logged in successfully");
            }
            else
            {
                return Unauthorized("Wrong user name or password");
            }
        }


        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, User user)//not includes password change!
        {
            string query = $"UPDATE users SET Name='{user.Name}', Role='{user.Role}' WHERE Id={id}";
            DbUtils.ExecuteNonQuery(query);
            return Ok("User updated successfully");
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
