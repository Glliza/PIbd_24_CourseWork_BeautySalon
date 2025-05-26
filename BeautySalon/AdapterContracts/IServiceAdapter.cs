using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.ViewModels;

namespace BeautySalon.AdapterContracts;

public interface IServiceAdapter
{
    ServiceR GetServiceById(string id);
    ServiceR GetServiceByName(string name);
    ServiceR CreateService(ServiceVM serviceModel);
    ServiceR UpdateService(ServiceVM serviceModel);
    ServiceR DeleteService(string id);
    ServiceR GetAllServices(bool onlyActive = true);
}