﻿namespace serverSide.Models;

public class CartItem
{
    public int CustomerId { get; set; }
    public int ItemId { get; set; }
    public int Amount { get; set; }
}


public class CartItemOut : Item{
    public int Amount { get; set; }
}