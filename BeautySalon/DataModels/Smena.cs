namespace BeautySalon.DataModels;

public class Smena
{
    public int ID { get; set; } 
    public int CashBoxID { get; set; }
    public int StaffID { get; set; } 
    public DateTime DateTimeStart { get; set; }
    public DateTime? DateTimeFinish { get; set; } 
}
