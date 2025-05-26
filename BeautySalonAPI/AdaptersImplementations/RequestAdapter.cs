using BeautySalon.AdapterContracts.OperationResponses;
using BeautySalon.BusinessLogicContracts;
using BeautySalon.AdapterContracts;
using BeautySalon.Infrastructure;
using BeautySalon.Exceptions;
using BeautySalon.DataModels;
using BeautySalon.ViewModels;
using AutoMapper;

namespace BeautySalonAPI.AdaptersImplementations;

public class RequestAdapter : IRequestAdapter
{
    private readonly IRequestBLC _requestBusinessLogic;
    private readonly ILogger<RequestAdapter> _logger;
    private readonly IMapper _mapper;

    public RequestAdapter(IRequestBLC requestBusinessLogic, ILogger<RequestAdapter> logger, IMapper mapper)
    {
        _requestBusinessLogic = requestBusinessLogic ?? throw new ArgumentNullException(nameof(requestBusinessLogic));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    private RequestR HandleNotFound(string message)
    {
        return OperationResponseBase.NotFound<RequestR>(message);
    }

    public RequestR GetRequestById(string id)
    {
        try
        {
            var requestDataModel = _requestBusinessLogic.GetRequestById(id);
            if (requestDataModel == null)
            {
                return HandleNotFound($"Request not found with id: {id}");
            }
            var requestViewModel = _mapper.Map<RequestVM>(requestDataModel);
            return OperationResponseBase.OK<RequestR, RequestVM>(requestViewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in GetRequestById");
            return OperationResponseBase.BadRequest<RequestR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in GetRequestById");
            return HandleNotFound($"Request not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetRequestById");
            return OperationResponseBase.InternalServerError<RequestR>(ex.Message);
        }
    }

    public RequestR CreateRequest(RequestVM requestModel)
    {
        try
        {
            var requestDataModel = _mapper.Map<RequestDM>(requestModel);
            if (requestDataModel == null)
            {
                _logger.LogError("Mapping failed from RequestViewModel to RequestDataModel in CreateRequest");
                return OperationResponseBase.BadRequest<RequestR>("Invalid request data provided.");
            }
            _requestBusinessLogic.InsertRequest(requestDataModel);
            var createdRequest = _requestBusinessLogic.GetRequestById(requestDataModel.ID);
            if (createdRequest == null)
            {
                return OperationResponseBase.NotFound<RequestR>($"Request not found with id: {requestModel.Id}");
            }
            var viewModel = _mapper.Map<RequestVM>(createdRequest);
            if (viewModel == null)
            {
                _logger.LogError("Mapping failed from RequestDataModel to RequestViewModel in CreateRequest");
                return OperationResponseBase.InternalServerError<RequestR>("Failed to map created request data.");
            }
            return OperationResponseBase.OK<RequestR, RequestVM>(viewModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in CreateRequest");
            return OperationResponseBase.BadRequest<RequestR>("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException in CreateRequest");
            return OperationResponseBase.BadRequest<RequestR>($"Incorrect data transmitted: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CreateRequest");
            return OperationResponseBase.InternalServerError<RequestR>(ex.Message);
        }
    }

    public RequestR UpdateRequest(RequestVM requestModel)
    {
        try
        {
            var requestDataModel = _mapper.Map<RequestDM>(requestModel);
            _requestBusinessLogic.UpdateRequest(requestDataModel);
            return OperationResponseBase.OK<RequestR, RequestVM>(requestModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException in UpdateRequest");
            return OperationResponseBase.BadRequest<RequestR>("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in UpdateRequest");
            return OperationResponseBase.NotFound<RequestR>($"Request not found with id: {requestModel.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpdateRequest");
            return OperationResponseBase.InternalServerError<RequestR>(ex.Message);
        }
    }

    public RequestR DeleteRequest(string id)
    {
        try
        {
            _requestBusinessLogic.DeleteRequest(id);
            return OperationResponseBase.NoContent<RequestR>();
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException in DeleteRequest");
            return OperationResponseBase.NotFound<RequestR>($"Request not found with id: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in DeleteRequest");
            return OperationResponseBase.InternalServerError<RequestR>(ex.Message);
        }
    }

    public RequestR GetAllRequests(bool onlyActive = true)
    {
        try
        {
            var requestDataModels = _requestBusinessLogic.GetAllRequests(onlyActive);
            var requestViewModels = _mapper.Map<List<RequestVM>>(requestDataModels);
            return OperationResponseBase.OK<RequestR, List<RequestVM>>(requestViewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAllRequests");
            return OperationResponseBase.InternalServerError<RequestR>(ex.Message);
        }
    }
}