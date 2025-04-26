namespace BeautySalon.DataModels;

public class Product
{
    public int ID { get; set; }
    public string Name { get; set; } 
    public string? Description { get; set; } 
    public int StockQuantity { get; set; } 
    public decimal BasePrice { get; set; }
}
