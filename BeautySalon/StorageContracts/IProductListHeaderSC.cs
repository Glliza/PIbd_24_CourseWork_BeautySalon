using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface IProductListHeaderSC
{
    Task<ProductListHeader?> GetElementByID(string id);
    Task AddElement(ProductListHeader headerDataModel);
    Task DelElement(string id); // Soft delete the header
    Task RestoreElement(string id);
    // You would NOT typically have GetList or methods to get *items* here.
    // Getting items would likely still be via the parent Visit/Request SC including the header.
}
