namespace BeautySalon.ViewModels;

public class VisitVM
{
    public required string Id { get; set; }
    public required string CustomerID { get; set; }
    public required string StaffID { get; set; }
    public required string ServiceListID { get; set; }
    public bool Status { get; set; }
    public required DateTime DateTimeOfVisit { get; set; }
    public decimal TotalPrice { get; set; }
}