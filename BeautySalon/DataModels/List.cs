
namespace BeautySalon.DataModels
{
    public class List(string id, string productId, float wholePrice)
    {
        public string Id { get; private set; } = id;
        public string ProductId { get; private set; } = productId;
        public float WholePrice { get; private set; } = wholePrice;
    }
}
