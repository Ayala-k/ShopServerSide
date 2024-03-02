using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public IActionResult CreateOrder(Order order)
    {
        order.Status = OrderStatus.OrderRecieved;

        //insert order
        string orderQuery = $"INSERT INTO orders (CustomerId, Status) VALUES ('{order.CustomerId}', '{order.Status.ToString()}')";
        int insertedOrderId = DbUtils.ExecuteNonQuery(orderQuery);

        //insert order items
        string selectCartItems = $"SELECT * FROM cart_items WHERE CustomerId={order.CustomerId}";
        List<CartItem> cartItems = DbUtils.ExecuteSelectQuery<CartItem>(selectCartItems);
        if (cartItems.Count == 0)
        {
            return BadRequest("No items in cart");
        }

        cartItems.ForEach(cartItem =>
        {
            OrderItem orderItem = new OrderItem() { 
            OrderId = insertedOrderId,
            ItemId = cartItem.ItemId,
            Amount = cartItem.Amount,
            PricePerItem=GetPricePerItem(cartItem.ItemId)
            };
            string insertOrderItemQuery = $"INSERT INTO order_items (OrderId,ItemId,Amount,PricePerItem) VALUES ({orderItem.OrderId},{orderItem.ItemId},{orderItem.Amount},{orderItem.PricePerItem})";
            DbUtils.ExecuteNonQuery(insertOrderItemQuery);
        });

        //delete cart items
        string deleteCartItemsQuery = $"DELETE FROM cart_items WHERE CustomerId={order.CustomerId}";
        DbUtils.ExecuteNonQuery(deleteCartItemsQuery);

        return Ok("Order created successfully");
    }


    [Authorize]
    [HttpGet("{customerId}")]
    public IActionResult GetCustomersOrders(int customerId)
    {
        string ordersQuery = $"SELECT * FROM orders WHERE CustomerId={customerId}";
        List<Order> ordersWithoutitems = DbUtils.ExecuteSelectQuery<Order>(ordersQuery);

        List<Object> orders= new List<Object>();
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

    private double GetPricePerItem(int itemId)
    {
        string query = $"SELECT * FROM items WHERE Id={itemId}";
        List<Item> item = DbUtils.ExecuteSelectQuery<Item>(query);
        return item[0].Price;
    }
}
