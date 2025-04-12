
namespace BeautySalon.DataModels;

public class Smena(string cashBoxId, string staffId)
{
    public string CashBoxId { get; private set; } = cashBoxId;
    public DateTime DateTimeStart { get; private set; } = DateTime.UtcNow;
    public string StaffId { get; private set; } = staffId;
    public DateTime DateTimeEnd { get; private set; } = DateTime.UtcNow;
}
