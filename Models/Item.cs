namespace serverSide.Models;

public enum InstrumentalCategory
{
    Wind,
    String,
    Keyboard,
    Percussion,
    Brass
}

public class Item
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public InstrumentalCategory Category { get; set;}
}
