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
        

        [HttpPost]
        public IActionResult AddUser(User user)
        {
            string query = $"INSERT INTO users (Id, Name, Role) VALUES ('{user.Id}', '{user.Name}', '{user.Role}')";
            DbUtils.ExecuteNonQuery(query);
            return Ok("User added successfully");
        }


        [HttpPut("{id}")]
        public IActionResult UpdateUser(string id, User user)
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
