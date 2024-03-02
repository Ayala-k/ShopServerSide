namespace serverSide.Models;

public class OrderItem
{
    public int OrderId { get; set; }
    public int ItemId { get; set; }
    public int Amount { get; set; }
    public double? PricePerItem { get; set; }
}
