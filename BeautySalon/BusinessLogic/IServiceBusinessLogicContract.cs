using BeautySalon.DataModels;

namespace BeautySalon.BusinessLogic;

public interface IServiceBusinessLogicContract
{
    List<ServiceDM> GetAllServices(bool onlyActive = true);
    List<ServiceDM> GetFilteredServices(
        string? name = null,
        int? minDurationMinutes = null,
        int? maxDurationMinutes = null,
        decimal? minBasePrice = null,
        decimal? maxBasePrice = null);
    ServiceDM GetServiceById(string id);
    ServiceDM GetServiceByName(string name);
    void InsertService(ServiceDM serviceDataModel);
    void UpdateService(ServiceDM serviceDataModel);
    void DeleteService(string id);
}
