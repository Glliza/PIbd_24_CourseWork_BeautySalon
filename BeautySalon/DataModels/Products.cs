
namespace BeautySalon.DataModels
{
    public class Products(string id, string naming, string note, int amount, float price)
    {
        public string Id { get; private set; } = id;
        public string Naming { get; private set; } = naming;
        public string Note { get; private set; } = note;
        public int Amount { get; private set; } = amount;
        public float Price { get; private set; } = price;
    }
}
