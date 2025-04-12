
namespace BeautySalon.DataModels;

public class Check(string id, List<GoodsList> products, List<GoodsList> service, string customerId, float summ)
{
    public string Id { get; private set; } = id;
    public List<GoodsList> ProductsId { get; private set; } = products;
    public List<GoodsList> ServicesId { get; private set; } = service;
    public string CustomerId { get; private set; } = customerId;
    public float Summ { get; private set; } = summ;
}
