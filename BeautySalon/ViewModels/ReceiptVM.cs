namespace BeautySalon.ViewModels;

public class ReceiptVM
{
    public required string Id { get; set; }
    public string? StaffID { get; set; }
    public string? CustomerID { get; set; }
    public DateTime DateIssued { get; set; }
    public bool IsCanceled { get; set; }
    public decimal TotalPrice { get; set; }
    public string? RequestID { get; set; }
    public List<ProductListItemVM>? Products { get; set; }
}