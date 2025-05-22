namespace BeautySalon.ViewModels;

public class ShiftVM
{
    public required string Id { get; set; }
    public required string StaffID { get; set; }
    public required string CashBoxID { get; set; }
    public required DateTime DateTimeStart { get; set; }
    public DateTime? DateTimeFinish { get; set; }
}