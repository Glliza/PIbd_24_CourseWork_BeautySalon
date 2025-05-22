using BeautySalon.Enums;

namespace BeautySalon.ViewModels;

public class ProductVM
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required decimal BasePrice { get; set; }
    public required int StockQuantity { get; set; }
    public required ProductType Type { get; set; }
    public string? Description { get; set; }
}