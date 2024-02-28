namespace serverSide.Models
{

    public enum OrderStatus
    {
        OrderRecieved,
        OrderSent,
        OrderDelivered
    }

    public class Order
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public OrderStatus? Status { get; set; }
    }
}
