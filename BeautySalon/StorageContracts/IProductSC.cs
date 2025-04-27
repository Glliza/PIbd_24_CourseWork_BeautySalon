using BeautySalon.DataModels;
using BeautySalon.Enums;

namespace BeautySalon.StorageContracts;

public interface IProductSC
{
    // Get a list of products with optional filtering
    // Filters might include Name, StockQuantity threshold, ProductType, IsDeleted
    Task<List<ProductDM>> GetList(
        bool onlyActive = true, // Filter by IsDeleted flag
        string? productID = null, // Filter by specific Product ID
        string? name = null,
        int? stockQuantityBelow = null, // Filter for low stock
        ProductType? type = null); // Filter by ProductType enum

    Task<ProductDM?> GetElementByID(string id);

    // Get a single product by its name (assuming names are unique for active products)
    Task<ProductDM?> GetElementByName(string name);

    Task AddElement(ProductDM productDataModel);

    Task UpdElement(ProductDM productDataModel);

    Task DelElement(string id);

    Task RestoreElement(string id);

    // Optional: Method to update stock quantity directly
    Task UpdateStockQuantityAsync(string productId, int quantityChange);
    // Pass positive for add, negative for remove
}