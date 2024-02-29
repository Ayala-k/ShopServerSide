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
        public int? Id { get; set; }
        public int CustomerId { get; set; }
        public OrderStatus? Status { get; set; }
    }
}
