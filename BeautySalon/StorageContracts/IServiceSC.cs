using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface IServiceSC
{
    // Get a list of services with optional filtering
    // Filters might include Name, Duration range, BasePrice range, IsDeleted
    Task<List<ServiceDM>> GetList(
        bool onlyActive = true, // Filter by IsDeleted flag
        string? serviceID = null, // Filter by specific Service ID
        string? name = null,
        int? minDurationMinutes = null,
        int? maxDurationMinutes = null,
        decimal? minBasePrice = null,
        decimal? maxBasePrice = null);

    Task<ServiceDM?> GetElementByID(string id);

    // Get a single service by its name (assuming names are unique for active services)
    Task<ServiceDM?> GetElementByName(string name); // [ ??? ]*

    Task AddElement(ServiceDM serviceDataModel);

    Task UpdElement(ServiceDM serviceDataModel);

    Task DelElement(string id);

    Task RestoreElement(string id);
}
