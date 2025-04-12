
namespace BeautySalon.DataModels;

public class Visit(string id, string orderId, string staffId, string customerId, float summ)
{
    public string Id { get; private set; } = id;
    public DateTime DateTime { get; private set; } = DateTime.UtcNow;
    public string OrderId { get; private set; } = orderId;
    public string StaffId { get; private set; } = staffId;
    public string CustomerId { get; private set; } = customerId;
    public float Summ { get; private set; } = summ;
}
