namespace BeautySalon.DataModels;

public class Check
{
    public int ID { get; set; }
    public int? VisitID { get; set; }
    public int CashBoxID { get; set; }
    public int CustomerID { get; set; }
    public DateTime DateTimeIssued { get; set; }
    public decimal TotalSum { get; set; } // Calculated from CheckItems (in DataAccess/DTOs)
}
