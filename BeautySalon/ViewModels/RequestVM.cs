using BeautySalon.Enums;

namespace BeautySalon.ViewModels;

public class RequestVM
{
    public required string Id { get; set; }
    public required string CustomerID { get; set; }
    public required DateTime DateCreated { get; set; }
    public required OrderStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public List<ProductListItemVM>? Products { get; set; }
    public List<ServiceListItemVM>? Services { get; set; }
}