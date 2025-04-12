namespace BeautySalon.DataModels
{
    public class ProductsIdList(string id, double fullPrice, List<Product> products)
    {
        public string Id { get; private set; } = id;

        // public string ProductId { get; private set; } = productId; >> in the Products fact. list:

        public List<Product> Products { get; private set; } = products;

        public double FullPrice { get; private set; } = fullPrice;
    }
}
