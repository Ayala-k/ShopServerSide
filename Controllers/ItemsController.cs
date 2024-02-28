using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using serverSide.Models;
using serverSide.Utils;

namespace serverSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllItems()
        {
            string query = "SELECT * FROM items";
            List<Item> items = DbUtils.ExecuteSelectQuery<Item>(query);
            return Ok(items);
        }


        [HttpGet("{id}")]
        public IActionResult GetItemById(string id)
        {
            string query = $"SELECT * FROM items WHERE Id='{id}'";
            List<Item> item = DbUtils.ExecuteSelectQuery<Item>(query);
            return Ok(item);
        }


        [HttpPost]
        public IActionResult AddItem(Item item)
        {
            string query = $"INSERT INTO items (Id, Name, Description, Price) VALUES ('{item.Id}', '{item.Name}', '{item.Description}', '{item.Price}')";
            DbUtils.ExecuteNonQuery(query);
            return Ok("Item added successfully");
        }


        [HttpPut("{id}")]
        public IActionResult UpdateItem(string id, Item item)
        {
            string query = $"UPDATE items SET Name='{item.Name}', Description='{item.Description}', Price='{item.Price}' WHERE Id='{id}'";
            DbUtils.ExecuteNonQuery(query);
            return Ok("Item updated successfully");
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteItem(string id)
        {
            string query = $"DELETE FROM items WHERE Id='{id}'";
            DbUtils.ExecuteNonQuery(query);
            return Ok("Item deleted successfully");
        }
    }
}
