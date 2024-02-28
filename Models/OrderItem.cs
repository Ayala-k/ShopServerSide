namespace serverSide.Models
{
    public class OrderItem
    {
        public string OrderId { get; set; }
        public string ItemId { get; set; }
        public int Amount { get; set; }
        public double? PricePerItem { get; set; }
    }
}
