
namespace BeautySalon.DataModels;

public class CashBox(string id, float money)
{
    public string Id { get; private set; } = id;
    public float Money { get; private set; } = money;
}
