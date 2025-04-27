using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface IServiceListHeaderSC
{
    Task<ServiceListHeader?> GetElementByID(string id);
    Task AddElement(ServiceListHeader headerDataModel);
    Task DelElement(string id);
}
