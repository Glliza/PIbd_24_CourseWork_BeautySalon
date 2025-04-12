
namespace BeautySalon.DataModels;

public class CashBox(string id, double money)
{
    public string Id { get; private set; } = id;
    public double Money { get; private set; } = money;
}
