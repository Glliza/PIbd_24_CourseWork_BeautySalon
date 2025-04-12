
namespace BeautySalon.DataModels;

public class Service(string id, string orderId, string staffId, string customerId, double price, DateTime startSession, DateTime endSession)
{
    public string Id { get; private set; } = id;
    public DateTime DateTimeStart { get; private set; } = startSession;
    public DateTime DateTimeEnd { get; private set; } = endSession;
    public string StaffId { get; private set; } = staffId;

    // public string CustomerId { get; private set; } = customerId;
    public double Price { get; private set; } = price;
}
