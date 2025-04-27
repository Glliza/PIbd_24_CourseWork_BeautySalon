using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface IServiceSC
{
    Task<List<ServiceDM>> GetList(
        bool onlyActive = true,
        string? name = null,
        int? minDurationMinutes = null,
        int? maxDurationMinutes = null,
        decimal? minBasePrice = null,
        decimal? maxBasePrice = null);
    Task<ServiceDM?> GetElementByID(string id);
    Task<ServiceDM?> GetElementByName(string name); 
    Task AddElement(ServiceDM serviceDataModel);
    Task UpdElement(ServiceDM serviceDataModel);
    Task DelElement(string id);
}
