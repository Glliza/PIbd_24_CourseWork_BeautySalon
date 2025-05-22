namespace BeautySalon.ViewModels;

public class ServiceListItemVM
{
    public required string Id { get; set; }
    public required string ServiceID { get; set; }
    public required int QuantityOrSessions { get; set; }
    public required int TotalItemDuration { get; set; }
    public required decimal TotalItemPrice { get; set; }
}