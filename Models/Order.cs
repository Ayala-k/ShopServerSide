﻿namespace serverSide.Models
{
    public class Order
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string? Status { get; set; }
    }
}