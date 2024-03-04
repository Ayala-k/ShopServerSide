using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using serverSide.Exceptions;
using serverSide.Models;
using serverSide.Utils;
using System.Security.Claims;

namespace serverSide.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    [Authorize]
    [HttpGet("{customerId}")]
    public IActionResult GetAllCustomersCartItems(int customerId)
    {
        try
        {
            string query = $"SELECT * FROM cart_items WHERE CustomerId={customerId}";
            List<CartItem> items = DbUtils.ExecuteSelectQuery<CartItem>(query);
            double totalPrice = items.Sum(item => item.Amount * GetPricePerItem(item.ItemId));
            return Ok(new { items = items, price = totalPrice });
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


    [Authorize]
    [HttpPost]
    public IActionResult AddToCart([FromBody] int itemId)
    {
        int userId=TokenUtils.ExtractUserId(User.Claims);
        Console.WriteLine(itemId);
        try
        {
            string query = $"INSERT INTO cart_items (CustomerId,ItemId,Amount) VALUES ({userId},{itemId},{1})";
            Console.WriteLine( query);

            try
            {
                DbUtils.ExecuteNonQuery(query);
                Console.WriteLine("after add",query);

            }
            catch (DataAlreadyExistsException)
            {
                string selectQuery = $"SELECT * FROM cart_items WHERE CustomerId={userId} AND ItemId={itemId}";
                List<CartItem> previousItem = DbUtils.ExecuteSelectQuery<CartItem>(selectQuery);
                int previousAmount = previousItem[0].Amount;
                string updateQuery = $"UPDATE cart_items SET Amount={previousAmount + 1} WHERE CustomerId={userId} AND ItemId={itemId}";
                DbUtils.ExecuteNonQuery(updateQuery);
            }
            return Ok(new { message = "Item added successfully" });
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


    [Authorize]
    [HttpPut("{CustomerId}/{ItemId}")]
    public IActionResult UpdateItem(int CustomerId, int ItemId, [FromBody] int amount)
    {
        try
        {
            string query = $"UPDATE cart_items SET Amount={amount} WHERE CustomerId={CustomerId} AND ItemId={ItemId}";
            DbUtils.ExecuteNonQuery(query);
            return Ok("Item updated successfully");
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


    [Authorize]
    [HttpDelete("{CustomerId}/{ItemId}")]
    public IActionResult DeleteFromCart(int CustomerId, int ItemId)
    {
        try
        {
            string query = $"DELETE FROM cart_items WHERE CustomerId={CustomerId} AND ItemId={ItemId}";
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

    private double GetPricePerItem(int itemId)
    {
        string query = $"SELECT * FROM items WHERE Id={itemId}";
        List<Item> item= DbUtils.ExecuteSelectQuery<Item>(query);
        return item[0].Price;
    }

}
