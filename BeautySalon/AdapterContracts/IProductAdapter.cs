using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.ViewModels;

namespace BeautySalon.AdapterContracts;

public interface IProductAdapter
{
    ProductR GetProductById(string id);
    ProductR GetProductByName(string name);
    ProductR CreateProduct(ProductVM productModel);
    ProductR UpdateProduct(ProductVM productModel);
    ProductR DeleteProduct(string id);
    ProductR UpdateStockQuantity(string productId, int quantityChange);
    ProductR GetAllProducts(bool onlyActive = true);
}