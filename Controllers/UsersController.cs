using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using serverSide.Data;
using serverSide.Models;
using System;
using System.Linq;

namespace serverSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        //private readonly MyDbContext _context;

        //public UsersController(MyDbContext context)
        //{
        //    _context = context;
        //}

        [HttpGet]
        public IActionResult GetAllUsers()
        {

            string connection_str = "Server=localhost; Database=shop_db;  UID=root;  PWD=1135;";

            //var users = _context.Users.ToList();
            string str = "";

            using (MySqlConnection connection = new MySqlConnection(connection_str))
            {
                connection.Open();
                Console.WriteLine("Hello, World!");

                using (MySqlCommand command = new MySqlCommand("SELECT * FROM users", connection))
                {

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["Name"]} - {reader["Id"]}");
                            str += $"{reader["Name"]} - {reader["Id"]}";
                        }
                    }
                }

            }
            return Ok(str);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            string connection_str = "Server=localhost; Database=shop_db;  UID=root;  PWD=1135;";

            //var users = _context.Users.ToList();
            string str = "";

            using (MySqlConnection connection = new MySqlConnection(connection_str))
            {
                connection.Open();
                Console.WriteLine("Hello, World!");

                using (MySqlCommand command = new MySqlCommand($"SELECT * FROM users WHERE Id={id}", connection))
                {

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["Name"]} - {reader["Id"]}");
                            str += $"{reader["Name"]} - {reader["Id"]}";
                        }
                    }
                }

            }
            return Ok(str);
        }

        //    [HttpPost]
        //    public IActionResult CreateUser(User user)
        //    {
        //        user.Id = Guid.NewGuid(); // Generate a new GUID for the user
        //        _context.Users.Add(user);
        //        _context.SaveChanges();
        //        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        //    }
    }
}



//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace serverSide.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UsersController : ControllerBase
//    {
//        [HttpGet]
//        public IActionResult GetAllUsers()
//        {
//            string[] users = new string[] { "user1", "user2", "user3" };
//            return Ok(users);
//        }
//    }
//}
