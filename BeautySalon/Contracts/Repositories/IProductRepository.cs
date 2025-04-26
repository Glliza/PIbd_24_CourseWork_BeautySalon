using BeautySalon.DataModels;
using BeautySalon.Enums;

namespace BeautySalon.Contracts.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetProductByName(string name);
    Task<IEnumerable<Product>> GetLowStockProducts(int threshold);
    Task<IEnumerable<Product>> GetProductsByType(ProductType type); // Use the enum
}