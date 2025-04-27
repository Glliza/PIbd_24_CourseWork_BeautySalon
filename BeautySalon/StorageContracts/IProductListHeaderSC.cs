using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface IProductListHeaderSC
{
    Task<ProductListHeader?> GetElementByID(string id);
    Task AddElement(ProductListHeader headerDataModel);
    Task DelElement(string id);
}
