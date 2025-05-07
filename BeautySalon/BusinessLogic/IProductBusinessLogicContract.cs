using BeautySalon.DataModels;
using BeautySalon.Enums;

namespace BeautySalon.BusinessLogic;

public interface IProductBusinessLogicContract
{
    List<ProductDM> GetAllProducts(bool onlyActive = true);
    List<ProductDM> GetFilteredProducts(
        string? name = null,
        int? stockQuantityBelow = null,
        ProductType? type = null);
    ProductDM GetProductById(string id);
    ProductDM GetProductByName(string name);
    void InsertProduct(ProductDM productDataModel);
    void UpdateProduct(ProductDM productDataModel);
    void DeleteProduct(string id);
    void UpdateStockQuantity(string productId, int quantityChange);
}
