
namespace BeautySalon.DataModels
{
    public class ServicesIdList(string id, string customerId, string orderId, double fullPrice, List<Service> services)
    {
        public string Id { get; private set; } = id;

        public string CustomerId { get; private set; } = customerId;

        // public string ServiceId { get; private set; } = serviceId; >> in the Services fact. list:

        public List<Service> Services { get; private set; } = services;

        public double FullPrice { get; private set; } = fullPrice;
    }
}
