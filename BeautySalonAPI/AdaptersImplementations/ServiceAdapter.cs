using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.BusinessLogicContracts;
using BeautySalon.AdapterContracts;
using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using BeautySalon.ViewModels;
using AutoMapper;

namespace BeautySalonAPI.AdaptersImplementations;

public class ServiceAdapter : IServiceAdapter
{
    private readonly IServiceBLC _serviceBusinessLogic;
    private readonly ILogger<ServiceAdapter> _logger;
    private readonly IMapper _mapper;

    public ServiceAdapter(IServiceBLC serviceBusinessLogic, ILogger<ServiceAdapter> logger, IMapper mapper)
    {
        _serviceBusinessLogic = serviceBusinessLogic ?? throw new ArgumentNullException(nameof(serviceBusinessLogic));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    private ServiceR HandleNotFound(string message)
    {
        return OperationResponseBase.NotFound<ServiceR>(message);
    }
    public ServiceR GetServiceById(string id)
    {
        try
        {
            var serviceDataModel = _serviceBusinessLogic.GetServiceById(id);
            if (serviceDataModel == null)
            {
                return HandleNotFound($"Service not found with id: {id}");
            }
            var serviceViewModel = _mapper.Map<ServiceVM>(serviceDataModel);
            return OperationResponseBase.OK<ServiceR, ServiceVM>(serviceViewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in GetServiceById");
            return OperationResponseBase.BadRequest<ServiceR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in GetServiceById");
            return HandleNotFound($"Service not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetServiceById");
            return OperationResponseBase.InternalServerError<ServiceR>(ex.Message);
        }
    }

    public ServiceR GetServiceByName(string name)
    {
        try
        {
            var serviceDataModel = _serviceBusinessLogic.GetServiceByName(name);
            if (serviceDataModel == null)
            {
                return HandleNotFound($"Service not found with name: {name}");
            }
            var serviceViewModel = _mapper.Map<ServiceVM>(serviceDataModel);
            return OperationResponseBase.OK<ServiceR, ServiceVM>(serviceViewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in GetServiceByName");
            return OperationResponseBase.BadRequest<ServiceR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in GetServiceByName");
            return HandleNotFound($"Service not found with name: {name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetServiceByName");
            return OperationResponseBase.InternalServerError<ServiceR>(ex.Message);
        }
    }

    public ServiceR CreateService(ServiceVM serviceModel)
    {
        try
        {
            var serviceDataModel = _mapper.Map<ServiceDM>(serviceModel);
            if (serviceDataModel == null)
            {
                _logger.LogError("Mapping failed from ServiceViewModel to ServiceDataModel in CreateService");
                return OperationResponseBase.BadRequest<ServiceR>("Invalid service data provided.");
            }
            _serviceBusinessLogic.InsertService(serviceDataModel);
            var createdService = _serviceBusinessLogic.GetServiceById(serviceDataModel.ID);
            if (createdService == null)
            {
                return OperationResponseBase.NotFound<ServiceR>($"Service not found with name: {serviceModel.Id}");
            }
            var viewModel = _mapper.Map<ServiceVM>(createdService);
            if (viewModel == null)
            {
                _logger.LogError("Mapping failed from ServiceDataModel to ServiceViewModel in CreateService");
                return OperationResponseBase.InternalServerError<ServiceR>("Failed to map created service data.");
            }
            return OperationResponseBase.OK<ServiceR, ServiceVM>(viewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in CreateService");
            return OperationResponseBase.BadRequest<ServiceR>("Data is empty");
        }
        catch (BeautySalon.Exceptions.ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException in CreateService");
            return OperationResponseBase.BadRequest<ServiceR>($"Incorrect data transmitted: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CreateService");
            return OperationResponseBase.InternalServerError<ServiceR>(ex.Message);
        }
    }

    public ServiceR UpdateService(ServiceVM serviceModel)
    {
        try
        {
            var serviceDataModel = _mapper.Map<ServiceDM>(serviceModel);
            _serviceBusinessLogic.UpdateService(serviceDataModel);
            return OperationResponseBase.OK<ServiceR, ServiceVM>(serviceModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in UpdateService");
            return OperationResponseBase.BadRequest<ServiceR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in UpdateService");
            return OperationResponseBase.NotFound<ServiceR>($"Service not found with id: {serviceModel.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpdateService");
            return OperationResponseBase.InternalServerError<ServiceR>(ex.Message);
        }
    }

    public ServiceR DeleteService(string id)
    {
        try
        {
            _serviceBusinessLogic.DeleteService(id);
            return OperationResponseBase.NoContent<ServiceR>();
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in DeleteService");
            return OperationResponseBase.NotFound<ServiceR>($"Service not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in DeleteService");
            return OperationResponseBase.InternalServerError<ServiceR>(ex.Message);
        }
    }

    public ServiceR GetAllServices(bool onlyActive = true)
    {
        try
        {
            var serviceDataModels = _serviceBusinessLogic.GetAllServices(onlyActive);
            var serviceViewModels = _mapper.Map<List<ServiceVM>>(serviceDataModels);
            return OperationResponseBase.OK<ServiceR, List<ServiceVM>>(serviceViewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAllServices");
            return OperationResponseBase.InternalServerError<ServiceR>(ex.Message);
        }
    }
}