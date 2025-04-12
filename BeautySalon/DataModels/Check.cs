
namespace BeautySalon.DataModels;

public class Check(string id, List<ProductsIdList>? products, List<ServicesIdList>? service, string customerId, double summ)
{
    public string Id { get; private set; } = id;
    public List<ProductsIdList>? ProductsId { get; private set; } = products;
    public List<ServicesIdList>? ServicesId { get; private set; } = service;
    public string CustomerId { get; private set; } = customerId;
    public double Summ { get; private set; } = summ;
}
