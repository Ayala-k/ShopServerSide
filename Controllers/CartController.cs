using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using serverSide.Models;
using serverSide.Utils;

namespace serverSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        [HttpGet("{customerId}")]
        public IActionResult GetAllCustomersCartItems(int customerId)
        {
            string query = $"SELECT * FROM cart_items WHERE CustomerId={customerId}";
            List<CartItem> items = DbUtils.ExecuteSelectQuery<CartItem>(query);
            return Ok(items);
        }


        [HttpPost]
        public IActionResult AddToCart(CartItem item)
        {
            string query = $"INSERT INTO cart_items (CustomerId,ItemId,Amount) VALUES ({item.CustomerId},{item.ItemId},{item.Amount})";
            Console.Write(query);
            try
            {
                DbUtils.ExecuteNonQuery(query);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                string selectQuery = $"SELECT * FROM cart_items WHERE CustomerId={item.CustomerId} AND ItemId={item.ItemId}";
                List<CartItem> previousItem = DbUtils.ExecuteSelectQuery<CartItem>(selectQuery);
                int previousAmount = previousItem[0].Amount;
                string updateQuery= $"UPDATE cart_items SET Amount={previousAmount+1} WHERE CustomerId={item.CustomerId} AND ItemId={item.ItemId}";
                DbUtils.ExecuteNonQuery(updateQuery);
            }
            return Ok("Item added successfully");
        }


        [HttpPut("{CustomerId}/{ItemId}")]
        public IActionResult UpdateItem(int CustomerId, int ItemId, [FromBody] int amount)
        {
            string query = $"UPDATE cart_items SET Amount={amount} WHERE CustomerId={CustomerId} AND ItemId={ItemId}";
            DbUtils.ExecuteNonQuery(query);
            return Ok("Item updated successfully");
        }


        [HttpDelete("{CustomerId}/{ItemId}")]
        public IActionResult DeleteFromCart(int CustomerId, int ItemId)
        {
            string query = $"DELETE FROM cart_items WHERE CustomerId={CustomerId} AND ItemId={ItemId}";
            DbUtils.ExecuteNonQuery(query);
            return Ok("Item deleted successfully");
        }

    }
}
