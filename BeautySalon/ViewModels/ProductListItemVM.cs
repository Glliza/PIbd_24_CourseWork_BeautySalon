
namespace BeautySalon.ViewModels;

public class ProductListItemVM
{
    public required string Id { get; set; }
    public required string ProductID { get; set; }
    public required int Quantity { get; set; }
    public required decimal ItemPrice { get; set; }
}