
namespace BeautySalon.DataModels;

public class Smena(string cashBoxId, string staffId, DateTime start, DateTime end)
{
    public string CashBoxId { get; private set; } = cashBoxId;
    public DateTime DateTimeStart { get; private set; } = start;
    public string StaffId { get; private set; } = staffId;
    public DateTime DateTimeEnd { get; private set; } = end;
}
