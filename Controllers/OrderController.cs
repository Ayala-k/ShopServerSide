using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using serverSide.Models;
using serverSide.Utils;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace serverSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
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

            cartItems.ForEach(cartItem =>
            {
                OrderItem orderItem = new OrderItem() { 
                OrderId = insertedOrderId,
                ItemId = cartItem.ItemId,
                Amount = cartItem.Amount,
                };
                string insertOrderItemQuery = $"INSERT INTO order_items (OrderId,ItemId,Amount) VALUES ({orderItem.OrderId},{orderItem.ItemId},{orderItem.Amount})";
                DbUtils.ExecuteNonQuery(insertOrderItemQuery);
            });

            //delete cart items
            string deleteCartItemsQuery = $"DELETE FROM cart_items WHERE CustomerId={order.CustomerId}";
            DbUtils.ExecuteNonQuery(deleteCartItemsQuery);

            return Ok("Order created successfully");
        }


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
                orderItems = ordersItems
            };
            orders.Add(orderObject);
            });

            return Ok(orders);
        }
    }
}
