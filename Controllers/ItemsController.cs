using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using serverSide.Exceptions;
using serverSide.Models;
using serverSide.Utils;

namespace serverSide.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public IActionResult GetAllItems()
    {
        try
        {
            string query = "SELECT * FROM items";
            List<Item> items = DbUtils.ExecuteSelectQuery<Item>(query);
            return Ok(items);
        }
        catch (DataNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InternalDataBaseException)
        {
            return StatusCode(500, "Internal Data Base Error");
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }


    [Authorize(Policy = "AdminPolicy")]
    [HttpGet("{id}")]
    public IActionResult GetItemById(int id)
    {
        try
        {
            string query = $"SELECT * FROM items WHERE Id={id}";
            List<Item> item = DbUtils.ExecuteSelectQuery<Item>(query);
            return Ok(item);
        }
        catch (DataNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InternalDataBaseException)
        {
            return StatusCode(500, "Internal Data Base Error");
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }


    [Authorize(Policy = "AdminPolicy")]
    [HttpPost]
    public IActionResult AddItem(Item item)
    {
        try
        {
            string query = $"INSERT INTO items (Name, Description, Price, Category) VALUES ('{item.Name}', '{item.Description}', '{item.Price}', '{item.Category}')";
            DbUtils.ExecuteNonQuery(query);
            return Ok("Item added successfully");
        }
        catch (DataAlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        }
        catch (InternalDataBaseException)
        {
            return StatusCode(500, "Internal Data Base Error");
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }


    [Authorize(Policy = "AdminPolicy")]
    [HttpPut("{id}")]
    public IActionResult UpdateItem(int id, Item item)
    {
        try
        {
            string query = $"UPDATE items SET Name='{item.Name}', Description='{item.Description}', Price='{item.Price}', Category='{item.Category}' WHERE Id={id}";
            DbUtils.ExecuteNonQuery(query);
            return Ok("Item updated successfully");
        }
        catch (DataAlreadyExistsException)
        {
            return Conflict("User name already exists");
        }
        catch (InternalDataBaseException)
        {
            return StatusCode(500, "Internal Data Base Error");
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }


    [Authorize(Policy = "AdminPolicy")]
    [HttpDelete("{id}")]
    public IActionResult DeleteItem(int id)
    {
        try
        {
            string query = $"DELETE FROM items WHERE Id={id}";
            DbUtils.ExecuteNonQuery(query);
            return Ok("Item deleted successfully");
        }
        catch (InternalDataBaseException)
        {
            return StatusCode(500, "Internal Data Base Error");
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }
}
