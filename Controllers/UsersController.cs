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
            List<User> users = DbUtils.ExecuteQuery<User>(query);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            string query = $"SELECT * FROM users WHERE Id={id}";
            List<User> user = DbUtils.ExecuteQuery<User>(query);
            return Ok(user);
        }
    }
}
