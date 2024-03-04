using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509;
using serverSide.Exceptions;
using serverSide.Models;
using serverSide.Utils;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace serverSide.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    [Authorize]
    [HttpPost]
    public IActionResult CreateOrder()
    {
        int userId = TokenUtils.ExtractUserId(User.Claims);
        try
        {
            int insertedOrderId = insertOrder(userId);
            insertOrderItems(userId, insertedOrderId);
            deleteCartItems(userId);
            return Ok( new { message= "Order created successfully" });
        }
        catch (DataNotFoundException ex)
        {
            return NotFound("No items in cart");
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

    private int insertOrder(int userId)
    {
        string orderQuery = $"INSERT INTO orders (CustomerId, Status) VALUES ('{userId}', '{OrderStatus.OrderRecieved.ToString()}')";
        int insertedOrderId = DbUtils.ExecuteNonQuery(orderQuery);
        return insertedOrderId;
    }

    private void insertOrderItems(int userId,int orderId)
    {
        string selectCartItems = $"SELECT * FROM cart_items WHERE CustomerId={userId}";
        List<CartItem> cartItems = DbUtils.ExecuteSelectQuery<CartItem>(selectCartItems);

        cartItems.ForEach(cartItem =>
        {
            OrderItem orderItem = new OrderItem()
            {
                OrderId = orderId,
                ItemId = cartItem.ItemId,
                Amount = cartItem.Amount,
                PricePerItem = GetPricePerItem(cartItem.ItemId)
            };
            string insertOrderItemQuery = $"INSERT INTO order_items (OrderId,ItemId,Amount,PricePerItem) VALUES ({orderItem.OrderId},{orderItem.ItemId},{orderItem.Amount},{orderItem.PricePerItem})";
            DbUtils.ExecuteNonQuery(insertOrderItemQuery);
        });
    }

    private void deleteCartItems(int userId)
    {
        string deleteCartItemsQuery = $"DELETE FROM cart_items WHERE CustomerId={userId}";
        DbUtils.ExecuteNonQuery(deleteCartItemsQuery);
    }


    [Authorize]
    [HttpGet("{customerId}")]
    public IActionResult GetCustomersOrders(int customerId)
    {
        try
        {
            string ordersQuery = $"SELECT * FROM orders WHERE CustomerId={customerId}";
            List<Order> ordersWithoutitems = DbUtils.ExecuteSelectQuery<Order>(ordersQuery);

            List<Object> orders = new List<Object>();
            ordersWithoutitems.ForEach(order =>
            {
                string orderItemsQuery = $"SELECT * FROM order_items WHERE OrderId={order.Id}";
                List<OrderItem> ordersItems = DbUtils.ExecuteSelectQuery<OrderItem>(orderItemsQuery);
                var orderObject = new
                {
                    orderDetails = order,
                    orderItems = ordersItems,
                    price = ordersItems.Sum(item => item.Amount * GetPricePerItem(item.ItemId))
                };
                orders.Add(orderObject);
            });

            return Ok(orders);
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

    private double GetPricePerItem(int itemId)
    {
        string query = $"SELECT * FROM items WHERE Id={itemId}";
        List<Item> item = DbUtils.ExecuteSelectQuery<Item>(query);
        return item[0].Price;
    }
}
