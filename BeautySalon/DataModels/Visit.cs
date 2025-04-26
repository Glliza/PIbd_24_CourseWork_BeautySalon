namespace BeautySalon.DataModels;
public class Visit
{
    public int ID { get; set; }
    public int? OrderID { get; set; }
    public int CustomerID { get; set; }
    public int StaffID { get; set; }
    public DateTime DateTimeOfVisit { get; set; }
    public int Status { get; set; } // 0=Not Completed, 1=Completed (as per your int decision)
}
