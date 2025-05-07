using System.Text.Json;
using BeautySalon.BusinessLogic;
using BeautySalon.DataModels;
using BeautySalon.Exceptions;
using BeautySalon.StorageContracts;
using Microsoft.Extensions.Logging;

namespace BeautySalon.BLImplementations;

internal class ServiceBusinessLogicContract(IServiceSC serviceStorageContract, ILogger logger) : IServiceBusinessLogicContract
{
    private readonly ILogger _logger = logger;
    private readonly IServiceSC _serviceStorageContract = serviceStorageContract;

    public List<ServiceDM> GetAllServices(bool onlyActive = true)
    {
        _logger.LogInformation("GetAllServices params: {onlyActive}", onlyActive);
        return _serviceStorageContract.GetList(onlyActive).GetAwaiter().GetResult()
            ?? throw new NullListException();
    }

    public List<ServiceDM> GetFilteredServices(
        string? name = null,
        int? minDurationMinutes = null,
        int? maxDurationMinutes = null,
        decimal? minBasePrice = null,
        decimal? maxBasePrice = null)
    {
        _logger.LogInformation(
            "GetFilteredServices params: {name}, {minDuration}-{maxDuration}, {minPrice}-{maxPrice}",
            name, minDurationMinutes, maxDurationMinutes, minBasePrice, maxBasePrice);

        if (minDurationMinutes.HasValue && maxDurationMinutes.HasValue &&
            minDurationMinutes > maxDurationMinutes)
        {
            throw new ValidationException("Min duration cannot be greater than max duration");
        }

        if (minBasePrice.HasValue && maxBasePrice.HasValue &&
            minBasePrice > maxBasePrice)
        {
            throw new ValidationException("Min price cannot be greater than max price");
        }

        var result = _serviceStorageContract.GetList(
            true, name, minDurationMinutes, maxDurationMinutes, minBasePrice, maxBasePrice)
            .GetAwaiter().GetResult();

        return result ?? throw new NullListException();
    }

    public ServiceDM GetServiceById(string id)
    {
        _logger.LogInformation("GetServiceById for {id}", id);

        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        if (!id.IsGuid())
        {
            throw new ValidationException("Service ID is not a valid GUID");
        }

        var result = _serviceStorageContract.GetElementByID(id).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(id);
    }

    public ServiceDM GetServiceByName(string name)
    {
        _logger.LogInformation("GetServiceByName for {name}", name);

        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var result = _serviceStorageContract.GetElementByName(name).GetAwaiter().GetResult();
        return result ?? throw new ElementNotFoundException(name);
    }

    public void InsertService(ServiceDM serviceDataModel)
    {
        _logger.LogInformation("New service data: {json}", JsonSerializer.Serialize(serviceDataModel));

        ArgumentNullException.ThrowIfNull(serviceDataModel);
        serviceDataModel.Validate();

        _serviceStorageContract.AddElement(serviceDataModel).GetAwaiter().GetResult();
    }

    public void UpdateService(ServiceDM serviceDataModel)
    {
        _logger.LogInformation("Update service data: {json}", JsonSerializer.Serialize(serviceDataModel));

        ArgumentNullException.ThrowIfNull(serviceDataModel);
        serviceDataModel.Validate();

        _serviceStorageContract.UpdElement(serviceDataModel).GetAwaiter().GetResult();
    }

    public void DeleteService(string id)
    {
        _logger.LogInformation("Delete service by id: {id}", id);

        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        if (!id.IsGuid())
        {
            throw new ValidationException("Service ID is not a valid GUID");
        }

        _serviceStorageContract.DelElement(id).GetAwaiter().GetResult();
    }
}
