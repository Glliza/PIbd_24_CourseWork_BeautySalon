using BeautySalon.DataModels;
using BeautySalon.Enums;

namespace BeautySalon.StorageContracts;

public interface IProductSC
{
    Task<List<ProductDM>> GetList(
        bool onlyActive = true, 
        string? productID = null,
        string? name = null,
        int? stockQuantityBelow = null,
        ProductType? type = null); 

    Task<ProductDM?> GetElementByID(string id);
    Task<ProductDM?> GetElementByName(string name);
    Task AddElement(ProductDM productDataModel);
    Task UpdElement(ProductDM productDataModel);
    Task DelElement(string id);
    Task RestoreElement(string id);
    Task UpdateStockQuantityAsync(string productId, int quantityChange);
}