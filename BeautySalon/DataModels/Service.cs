namespace BeautySalon.DataModels;

public class Service
{
    public int ID { get; set; }
    public string Name { get; set; } 
    public string? Description { get; set; } // описание (не обязательно)
    public decimal BasePrice { get; set; } 
    public int? DurationMinutes { get; set; } 
}
